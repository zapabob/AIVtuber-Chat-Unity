AIVtuber Chat Application (C# + Unity + Dify API + Voicevox)
このリポジトリは、C# (Unity) を用いて開発中の「AIVtuberチャットアプリ」にゃ。
Dify APIを活用してLLM（外部/ローカルggufモデル）に問い合わせ、VRMアバターが自然な会話を可能にする。さらに、Voicevoxによる音声合成で、アバターが実際に声で応答してくれるから、まるで本物のVTuberと会話しているような没入感が得られるんだにゃ！
現在はWindows/macOS向けをターゲットとしているけど、将来的にAndroid対応も予定しているにゃ。

主な特徴
Dify API連携:
オンライン時は外部LLM、オフライン時はローカルggufモデルへDify API経由で問い合わせるハイブリッド構成。
ネット状況に応じて選べるフレキシブルな会話環境。

Voicevox音声合成:
テキストレスポンスをVoicevoxで音声化し、アバターが実際に“しゃべる”！
話者IDや音声パラメータ調整でカスタムボイスを楽しもう。

VRMアバター統合:
好きなVRMファイルをロードしてアバター化。
リップシンク、表情変化、アイドルモーションで魅力的なキャラクター体験。

直感的カスタマイズUI:
チャットフォームの外観、フォント、背景画像、BGM、Voicevoxの話者設定、LLMパラメータなどを自由にカスタム可能。
シンプルなUIで自分好みのAIVtuber空間を構築できる。

クロスプラットフォーム（将来的計画）:
初期リリースはWindows/macOS対応。
Android対応は今後の追加実装で検討しているにゃ。

必要なもの
Unity 2022 LTS推奨
C#（Unity標準ランタイム）
Dify APIキー（外部LLM利用時）
ローカルggufモデルファイル（ローカルLLM利用時）
Voicevoxエンジン（ローカルにVoicevox HTTPサーバー立ち上げ可能な環境）
VRM形式のアバターモデルファイル
セットアップ手順
リポジトリクローン:

bash
コードをコピーする
git clone https://github.com/<YourUserName>/AIVtuberChatApp.git
cd AIVtuberChatApp
Unityでプロジェクトを開く:
Unity HubからAIVtuberChatAppプロジェクトを読み込む。
推奨：Unity 2022 LTS + URP環境

Dify APIキー & Voicevox設定:

Assets/Settings ディレクトリ配下に APIKeys.json (仮)でDify APIキー設定。
Voicevox用にはローカルでVoicevoxエンジン(HTTPサーバ)起動。 AppSettings.json (仮)でVoicevoxサーバーURLや話者ID初期値を設定。
ローカルLLMモデルの配置:
Assets/LocalModels にggufモデルファイルを配置。Difyローカルモードで呼び出せるように事前準備。

VRMファイルの準備:
アプリ起動後、SettingsメニューからVRMファイルを読み込むか、StreamingAssets/Avatars ディレクトリに配置しておく。

ビルド手順
Windows/macOS:
Unityエディタ上でFile > Build Settingsからプラットフォーム選択＆Build実行。

Android (将来対応計画):
将来、Androidビルド用設定ガイドを整備予定。
現時点ではAndroid対応は未実装、計画段階。

使い方（基本フロー）
アプリを起動
Settings画面でDify APIキー入力、LLMモード選択（オンライン/ローカルgguf）、Voicevox話者選択
VRMアバター読込、背景画像やBGM、フォント設定を自由にカスタム
チャット画面へ戻り、テキスト入力→送信ボタンでAIに話しかける
LLM応答テキストが返ってきたらVoicevoxで音声生成→アバターが声で応答＆リップシンク
必要に応じてチャットログ保存、過去ログ参照など
カスタマイズ・拡張
Assets/UI 以下でUIレイアウト、スタイルを調整可能
Assets/Scripts/LLM 以下でDify API呼び出しやローカルモデル呼び出し処理を拡張
Voicevox話者IDや音声パラメータ変更でキャラクターの印象を変えられる
開発状況と計画
現状: 基本機能（LLM応答取得、Voicevox音声出力、VRMリップシンク）を実装中
今後の計画:
Android対応検討
より高度な感情分析による表情制御
プラグインシステムによるカスタムモーションや追加機能の拡張
コントリビュート方法
バグ報告や機能リクエストはIssueで受付中
プルリクエストも歓迎
コントリビュートガイドラインはCONTRIBUTING.mdに後日記載予定
