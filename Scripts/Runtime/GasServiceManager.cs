using UniGasClient.Data;
using UniGasClient.Service;
using UniGasClient.Http;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UniGasClient.Editor;

namespace UniGasClient
{
    public class GasServiceManager : MonoBehaviour
    {
        public static GasServiceManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }


        private GasLoggerService loggerService;
        private RankingClientService rankingService;


        private void Start()
        {
            var gasSettings = GasSettingsService.LoadSettings();
            var httpClient = new GasHttpClient(gasSettings);

            loggerService = new GasLoggerService(httpClient);
            rankingService = new RankingClientService(httpClient);
        }

        public async Task SendLog<T>(T datas, string sheetName = null)
        {
            await loggerService.SendLog(datas, sheetName);
        }

        public async Task SendLog(Dictionary<string, object> datas, string sheetName = null)
        {
            await loggerService.SendLog(datas, sheetName);
        }

        public async Task<RankingResponse> GetTop5Ranking(string sheetName = null)
        {
            return await rankingService.GetTop5Ranking(sheetName);
        }

        public async Task<ScoreRankResponse> GetScoreRanking(BigInteger score, string sheetName = null)
        {
            return await rankingService.GetScoreRank(score, sheetName);
        }
    }
}
