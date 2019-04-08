using Partiality.Modloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using On;
using System.Reflection;
using MonoMod.Utils;

namespace SimpleItemEditTemplate
{
    //Our Script
    public class ScriptLoad : MonoBehaviour
    {
        public void Initialise()
        {
            //Don't forget to call the Patch method so you actually hook into methods
            Patch();   
        }

            //Here is where Partialitys real magic is
        public void Patch()
        {         
            //We Hook our new sexy method on to the Developers dirty old one
            //The Best way to find these methods is through dnspy or ilspy
            //Find the class most likely to deal with whatever you are changing
            //then find a method that looks suitable to hook into
            //if you dont use unity this may cause some problems
            //You want to try avoid using the ctor or Awake method unless you have a specific reason
            //Most things will still be loading in those few seconds difference 
            //you will get a lot of NullReferenceExceptions because you are trying to access something before its fully loaded
            //In this case I'm hooking onto ItemAssetsDoneLoading seems like a nice one

            //It's best not to copy the below line but write it yourself
            //once you get to the 'new' part press tab and let Visual Studio auto complete
            //On.CLASS.METHOD += new 
            //to
            //On.Character.ActionPerformed += new On.Character.hook_ActionPerformed(functionHook)

            //Notice the last variable functionHook, you can name this whatever you want, this is the name of the method you will be using in the hook
            //type any name you want, right click it click quick actions and refactoring and generate method 

            //On.ItemManager.ItemAssetsDoneLoading += new On.ItemManager.hook_ItemAssetsDoneLoading(itemAssetsLoadedHook);

            On.ResourcesPrefabManager.LoadItemPrefabs += new On.ResourcesPrefabManager.hook_LoadItemPrefabs(loadItemPrefabsHook);
        }

        //Here's the method
        private void loadItemPrefabsHook(On.ResourcesPrefabManager.orig_LoadItemPrefabs orig, ResourcesPrefabManager self)
        {
            orig(self);
            //Be sure to call the original method you are hooking on to
            //99% of the time you will want to call it before you do anything yourself, failing to call this will stop whatever class you are hooking into from loading correctly
            //which could cause a lot of other problems
            orig(self);

            //WE ARE HERE CODING TIME
            //Firstly Here's the ID's Of two Items
            //5110110_PistolHandCannon
            //3000300_BigHatArmorHelmBlue

            //I'm going to be usnig the PistolHandCannon because PistolHandCannon is a PistolHandCannon
            var item = self.GetItemPrefab("5110110");

            //Now we'll debug the item
            Debug.Log("ITEM");
            Debug.Log(item);


            //Since I want this PistolHandCannon to also give me a metric ton of magic reduction and the variable for this is private
            //We're going to have to use Reflection (yaaay :|)


            //Firstly we need the Type of the class we want the stat from
            //In this case and the case of probably all weapons is
            //EquipmentStats
            //myType = EquipmentStats now essentially
            Type myType = typeof(EquipmentStats);


            //then we need to use a function from the Reflection class
            //the first variable we need for this function is the variable we want to change but currently aren't allowed
            //in my case it's 
            //m_manaUseModifier
            //the second are bindings for the reflection in most cases how they are set below should work fine since you wont need reflection if the vars are public
            FieldInfo field = myType.GetField("m_manaUseModifier", BindingFlags.NonPublic | BindingFlags.Instance);

            //Here I cache a reference to the component as doing GetComponent too often can seriously drag Unity and therefore the game down
            EquipmentStats equipStatComp = item.GetComponent<EquipmentStats>();

            //Here I am using the field variable and passing a reference to this items EquipmentStat Component
            //how do I know it had one? well I used ListComponentsInDebug which is a handy way to get all the components on a object in unity
            //because don't forget we are editing a Prefab here, which is basically the "mastercopy" of an item/object etc
            //if something needs to spawn a PistolHandCannon in it will ask for the prefab which contains the Model, Scripts, Textures references for the item/object.
            field.SetValue(equipStatComp, -100f);

            //and if you check consoleLog.txt or output_log.txt you will see the value is now -100 and it even shows in game on the tooltip
            Debug.Log(field.GetValue(equipStatComp));
        }


        //Once you find a component you think relevant to your pursuits
        //you can check its methods, fields, properties through dnspy or ilspy
        public void ListComponentsInDebug(GameObject obj)
        {
            Component[] components = obj.GetComponents<Component>();
            Debug.Log("COMPONENTS");
            foreach (var c in components)
            {
                Debug.Log(c);
            }
        }
       
    }

}
