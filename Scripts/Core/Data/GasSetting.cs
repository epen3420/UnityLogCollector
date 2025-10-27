using UnityEngine;

namespace UniGasClient.Data
{
    [CreateAssetMenu(fileName = "NewGasSettings", menuName = "GasSettings")]
    internal class GasSettings : ScriptableObject
    {
        [SerializeField]
        private string authToken;
        [SerializeField]
        private string deployId;
        [SerializeField]
        private string sheetId;

        public string AuthToken => authToken;
        public string DeployId => deployId;
        public string GasUrl => $"https://script.google.com/macros/s/{deployId}/exec";
        public string SheetId => sheetId;

        public void Init(string authToken, string deployId, string sheetId)
        {
            this.authToken = authToken;
            this.deployId = deployId;
            this.sheetId = sheetId;
        }
    }
}
