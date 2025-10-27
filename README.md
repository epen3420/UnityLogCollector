# UniGasClient (Unity GAS Client)

**UniGasClient** は、UnityからGoogle Apps Script (GAS) への非同期通信を簡単に行うためのライブラリです。

ゲームのログをスプレッドシートに送信（`doPost`）したり、スプレッドシートからランキングデータを取得（`doGet`）したりする機能を、最適化されたシンプルなAPIで提供します。

## 🚀 特徴 (Features)

  * **非同期通信**: `async/await` ベースで、GASへのリクエストをメインスレッドをブロックせずに実行します。
  * **ログ送信**: クラス (`<T>`) や `Dictionary` 形式のデータをGASに送信し、スプレッドシートに記録します。
  * **ランキング取得**: GASからトップNランキングや特定のスコアの順位をJSONで取得します。
  * **簡単な設定**: 専用の `EditorWindow` から、GASのデプロイIDや認証キーを安全に設定できます。

## 📦 導入方法 (Installation)

### 1\. (推奨) Unity Package Manager (UPM)

Unityエディタの `Window > Package Manager` を開き、「+」ボタンから `Add package from git URL...` を選択し、このリポジトリのURLを入力します。

```
https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git
```

### 2\. Git Submodule

プロジェクトの `Assets` フォルダ配下（例: `Assets/Plugins`）で、以下のコマンドを実行します。

```bash
git submodule add https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git Assets/Plugins/UniGasClient
```

## ⚙️ セットアップ (Configuration)

ライブラリを使用するには、**GAS側**と**Unity側**の両方で設定が必要です。

### 1\. Google Apps Script (GAS) 側の設定

1.  Google Driveで新しいスプレッドシートを作成します。
2.  `拡張機能 > Apps Script` を開き、このライブラリが要求する `doPost`（ログ受信用）および `doGet`（ランキング送信用）関数を実装します。
3.  **認証トークンを設定します**:
      * GASエディタの `プロジェクトの設定（歯車アイコン） > スクリプト プロパティ` を開きます。
      * `AUTH_TOKEN` というキー名で、セキュアな認証トークン（パスワード）を設定します。
4.  **Webアプリとしてデプロイします**:
      * `デプロイ > 新しいデプロイ` を選択します。
      * 種類は「Webアプリ」を選択します。
      * アクセスできるユーザーを「**全員**」に設定します。
      * デプロイ後、**デプロイID**をコピーします。

### 2\. Unity 側の設定

1.  Unityエディタの上部メニューから `Tools > LogSenderSetting` を選択します。
2.  専用の `EditorWindow` が開かれ、ライブラリフォルダ内に設定ファイル（`GasSettings.asset`）が自動的に作成されます。
3.  ウィンドウに、GAS側で取得した情報を入力します。
      * **GAS URL (Deploy ID)**: GASのデプロイID
      * **Auth Token**: GASで設定した認証トークン
      * **Sheet ID**: ログを書き込むスプレッドシートのID（URLから取得）
4.  `Save Settings to Disk` ボタンを押して保存します。

### 🚨【最重要】秘密鍵の保護

`GasSettingEditor` が作成する `GasSettings.asset` ファイルには、**認証トークンなどの秘密鍵**が含まれます。

**このファイルを絶対にGitHubにコミットしないでください。**

プロジェクトのルートにある `.gitignore` ファイルに、`GasSettings.asset` のパスを追記してください。

```gitignore
# UniGasClient Settings
GasSettings.asset
GasSettings.asset.meta
```

## 📖 使い方 (Usage)

シーンに `UniGasClient` のプレハブを配置するか、`GasServiceManager.cs` をアタッチしたGameObjectを配置してください。シングルトン (`Instance`) 経由で各機能にアクセスできます。

### ログを送信する (SendLog)

**`GasServiceManager.Instance.SendLog(data, sheetName)`**

`sheetName` は、GAS側でシート名によって処理を分岐させたい場合に使用します（任意）。

```csharp
using UnityEngine;
using UniGasClient; // GasServiceManager を使うため
using System.Collections.Generic;

public class MyGameController : MonoBehaviour
{
    // クラス (DTO) を送信する例
    [System.Serializable]
    public class PlayLog
    {
        public string PlayerName;
        public int Score;
        public bool IsClear;
    }

    public async void OnGameEnd(int finalScore)
    {
        var log = new PlayLog
        {
            PlayerName = "Hero",
            Score = finalScore,
            IsClear = true
        };

        // UniGasClient.Core アセンブリの型を参照
        await GasServiceManager.Instance.SendLog(log, "Stage1Clear");
    }

    // Dictionary を送信する例
    public async void OnGameStart()
    {
        var log = new Dictionary<string, object>
        {
            { "Event", "GameStart" },
            { "Timestamp", System.DateTime.Now.ToString() }
        };

        await GasServiceManager.Instance.SendLog(log, "EventLog");
    }
}
```

### ランキングを取得する (Get Ranking)

GAS側で `RankingResponse` または `ScoreRankResponse` のJSON形式で応答が返されることを想定しています。

```csharp
using UnityEngine;
using UniGasClient;
using UniGasClient.DTO; // RankingResponse を使うため
using System.Numerics;

public class RankingViewController : MonoBehaviour
{
    // トップ5ランキングを取得する例
    public async void ShowTop5Ranking()
    {
        // "RankingSheet" はGAS側で処理するシート名
        RankingResponse response = await GasServiceManager.Instance.GetTop5Ranking("RankingSheet");

        if (response != null)
        {
            foreach (var entry in response.ranking)
            {
                Debug.Log($"{entry.name}: {entry.score}");
            }
        }
    }

    // 自分のスコアが何位か問い合わせる例
    public async void CheckMyRank(BigInteger myScore)
    {
        ScoreRankResponse response = await GasServiceManager.Instance.GetScoreRanking(myScore, "RankingSheet");

        if (response != null)
        {
            Debug.Log($"Your score {response.score} is Rank {response.rank}!");
        }
    }
}
```

## 📜 ライセンス (License)

このプロジェクトは [MIT License](https://www.google.com/search?q=LICENSE) のもとで公開されています。
