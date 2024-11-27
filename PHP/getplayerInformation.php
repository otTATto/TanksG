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

// POSTデータを取得
if (!isset($_POST['id'])) {
    die("Invalid input");
}

$id = intval($_POST['id']); // 安全のため整数にキャスト

// SQLクエリでプレイヤー名を取得
$sql = "SELECT playername FROM ranking WHERE id = $id";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    echo $row['playername']; // Unityに返す
} else {
    echo "Player not found"; // プレイヤーが見つからない場合
}

$conn->close();
?>