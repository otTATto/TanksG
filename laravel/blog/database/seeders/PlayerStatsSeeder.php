<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class PlayerStatsSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // ゲームユーザーのマスターデータ
        $playerstats = [
            [
                'id' => 2,
                'game_user_id' => 2,
                'wincount' => 0,
                'losecount' => 3,
            ],
            [
                'id' => 3,
                'game_user_id' => 3,
                'wincount' => 3,
                'losecount' => 3,
            ],
            [
                'id' => 4,
                'game_user_id' => 4,
                'wincount' => 10,
                'losecount' => 3,
            ],
            [
                'id' => 5,
                'game_user_id' => 5,
                'wincount' => 110,
                'losecount' => 3,
            ],
            [
                'id' => 6,
                'game_user_id' => 6,
                'wincount' => 10,
                'losecount' => 13,
            ],
            [
                'id' => 7,
                'game_user_id' => 7,
                'wincount' => 110,
                'losecount' => 113,
            ],
            [
                'id' => 8,
                'game_user_id' => 8,
                'wincount' => 80,
                'losecount' => 3,
            ],
            [
                'id' => 9,
                'game_user_id' => 9,
                'wincount' => 100,
                'losecount' => 3,
            ],
            [
                'id' => 10,
                'game_user_id' => 10,
                'wincount' => 100,
                'losecount' => 10,
            ],
            [
                'id' => 11,
                'game_user_id' => 11,
                'wincount' => 100,
                'losecount' => 12,
            ],
            [
                'id' => 12,
                'game_user_id' => 12,
                'wincount' => 100,
                'losecount' => 3,
            ],
            [
                'id' => 13,
                'game_user_id' => 13,
                'wincount' => 100,
                'losecount' => 11,
            ],
        ];

        // トランザクションを利用してデータ登録
        DB::transaction(function () use ($playerstats) {
            foreach ($playerstats as $playerstat) {
                // 勝率を動的に計算
                $totalGames = $playerstat['wincount'] + $playerstat['losecount'];
                if($totalGames > 0) 
                $playerstat['winrate'] = round(($playerstat['wincount'] / $totalGames) * 100, 2);
                else $playerstat['winrate'] = 0;

                // データベースに登録
                \App\Models\PlayerStat::create($playerstat);
            }
        });
    }
}