using Partiality.Modloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace SimpleItemEditTemplate
{
    public class SimpleItemEditTemplate : PartialityMod
    {
        public static ScriptLoad script;

        public SimpleItemEditTemplate()
        {
            this.ModID = "SimpleItemEdit";
            this.Version = "0.1";
            this.author = "Author";

        }

        public override void OnEnable()
        {
            base.OnEnable();


            //Create GameObject in Unity
            GameObject obj = new GameObject();


            //Add our script to it
            //This is why we Inherit from MonoBehaviour only MonoBehaviours(Or classes derived from it) can be components
            script = obj.AddComponent<ScriptLoad>();

            //Call the Initalise function of our script, you may not need this but its here anyway
            script.Initialise();
            Debug.Log("::" + this.ModID + " has loaded " + "::");
        }



    }


    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
    }
}
