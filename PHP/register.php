<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "users";

// データベース接続
$conn = new mysqli($servername, $username, $password, $dbname);

// 接続エラー確認
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// プリペアドステートメントでSQLインジェクション対策
$stmt = $conn->prepare("INSERT INTO ranking (playername) VALUES (?)");
$stmt->bind_param("s", $username);

// ユーザー名を設定
$username = "NONAME";

// クエリ実行と結果の判定
if ($stmt->execute()) {
    $last_id = $conn->insert_id;
    echo "success:" . $last_id;
} else {
    echo "Error: " . $stmt->error;
}

// ステートメントと接続のクローズ
$stmt->close();
$conn->close();
?>
