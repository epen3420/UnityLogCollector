using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

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

    [SerializeField]
    private GasSettings gasSettings;

    private GasLoggerService loggerService;
    private RankingClientService rankingService;

    private void Start()
    {
        if (gasSettings == null)
        {
            Debug.LogError("GasSettings is null");
        }

        GasHttpClient httpClient = new GasHttpClient(gasSettings);

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
