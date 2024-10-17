using Newtonsoft.Json;
using SaveSystem.Runtime.DataStructure;

namespace Plugins.SaveSystem.DataStructure
{
    public class ClassData : BaseData<object>
    {
        public ClassData()
        {
            
        }
        
        public ClassData(ClassData data) : base(data.Data)
        {
            
        }

        public override string ToString() => 
            JsonConvert.SerializeObject(this, SaveData.JsonSerializerSettings);
    }
}