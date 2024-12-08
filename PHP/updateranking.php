<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "users";

// データベース接続
$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    echo json_encode(["error" => "Connection failed: " . $conn->connect_error]);
    exit;
}



$id = intval($_POST['id']);
$ifplayerwin = intval($_POST['ifplayerwin']);

// 勝敗データの更新
if ($ifplayerwin == 1) {
    $update_sql = "UPDATE ranking SET wincount = wincount + 1 WHERE id = ?";
} elseif ($ifplayerwin == 0) {
    $update_sql = "UPDATE ranking SET losecount = losecount + 1 WHERE id = ?";
} else {
    echo json_encode(["error" => "Invalid value for ifplayerwin"]);
    exit;
}

$stmt = $conn->prepare($update_sql);
$stmt->bind_param("i", $id);

if (!$stmt->execute()) {
    echo json_encode(["error" => "Error updating score: " . $stmt->error]);
    exit;
}

// ランキングを取得
$ranking_sql = "SELECT 
                    id, 
                    playername, 
                    FLOOR(CASE 
                        WHEN (wincount + losecount) = 0 THEN 0 
                        ELSE (wincount / (wincount + losecount)) * 100 
                    END) AS winrate, 
                    wincount, 
                    losecount 
                FROM ranking 
                ORDER BY winrate DESC 
                LIMIT 10";

$result = $conn->query($ranking_sql);

if ($result->num_rows > 0) {
    $ranking = [];
    while ($row = $result->fetch_assoc()) {
        $ranking[] = $row;
    }
    echo json_encode(["players" => $ranking]);
} else {
    echo json_encode(["players" => []]);
}

$conn->close();
?>

