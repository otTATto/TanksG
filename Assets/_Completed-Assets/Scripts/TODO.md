# 砲弾ストック数の仕様
- [x] ゲーム開始時点で砲弾ストック数は10とする。ストック数の上限は50とする
    - [x] ストック数のフィールドをTankshoting.csに追加
- [x] ストック数は、HUDとして画面左上にアイコン表示する。ストック数1とストック数10のアイコンを活用し、ストック数の視認性を高める
    - [x] シーンにアイコンを追加
    - [x] /Scripts/UI下にHUDを管理するスクリプトhud.csを追加
- [x] 砲弾を発射するたびにストック数は1ずつ減り、ストック数が0になると砲弾は発射できない
    - [x] Tankshooting.csのFire()メソッドを呼び出す部分を変更

# 砲弾カートリッジの仕様
- [x] フィールド上のランダムな場所に、ある一定の時間間隔で砲弾カートリッジを出現させる
- [x] 砲弾カートリッジは、出現してから一定の時間が過ぎたタイミングで点滅をはじめ、さらに時間が経過すると自動消滅する
- [x] 戦車の車体に砲弾カートリッジが触れることで砲弾ストック数が10増える

# 砲弾の飛距離ゲージの仕様
- [x] SPACEキーを押すことで飛距離ゲージを表示し、押しっぱなしにしている間飛距離ゲージは最小から最大までを反復する
- [x] SPACEキーを離すことで砲弾を発射する。その時に飛距離ゲージの大きさに合わせて砲弾の飛距離が変化する

# 対戦車地雷ストック数の仕様
- [x]ゲーム開始時点で対戦車地雷ストック数は0とする。ストック数の上限は3とする
- [x]ストック数は、HUDとして画面左上にアイコン表示する。ストック数1のアイコンを表示し、ストック数の視認性を高める
- []対戦車地雷を設置するたびにストック数は1ずつ減り、ストック数が0になると設置できない

# 対戦車地雷カートリッジの仕様
- [x]フィールド上のランダムな場所に、ある一定の時間間隔で対戦車地雷カートリッジを出現させる
- [x]対戦車地雷カートリッジは、出現してから一定の時間が過ぎたタイミングで点滅をはじめ、さらに時間が経過すると自動消滅する
- [x]戦車の車体に対戦車地雷カートリッジが触れることでストック数が1増える

# 対戦車地雷の仕様
- [x]ENTERキーを押下すると、自機の現在位置に対戦車地雷が設置される。設置する際、自機はその場に一定時間、強制停止させられる。その間、自機の車体や砲塔を動かすことはできない
- []設置された対戦車地雷の位置にはドクロマークが表示される
- []対戦車地雷は、砲弾や戦車の車体（自機も含む）に接触したタイミングで爆発する。戦車は爆発と爆風により、爆心地からの距離に応じたダメージを被ると爆風により、爆心地から の距離に応じたダメージを被る