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

// POSTデータの存在確認
if (!isset($_POST['id']) || !isset($_POST['playername'])) {
    die("Invalid input");
}

$id = intval($_POST['id']); // IDを整数に変換して安全性を確保
$name = $_POST['playername'];

// プリペアドステートメントを使用
$stmt = $conn->prepare("UPDATE ranking SET playername = ? WHERE id = ?");
if (!$stmt) {
    die("Prepare failed: " . $conn->error);
}
$stmt->bind_param("si", $name, $id);

// クエリ実行と結果の判定
if ($stmt->execute()) {
    echo "success";
} else {
    echo "Error: " . $stmt->error;
}

// ステートメントと接続のクローズ
$stmt->close();
$conn->close();
?>
