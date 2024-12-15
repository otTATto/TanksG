<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

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
                'losecount' => 3
            ],
            [
                'id' => 3,  
                'game_user_id' => 3, 
                'wincount' => 3,
                'losecount' => 3
            ],
            [
                'id' => 4,
                'wincount' => 10,
                'losecount' => 3
            ],
            [
                'id' => 5,  
                'wincount' => 110,
                'losecount' => 3
            ],
            [
                'id' => 6,  
                'wincount' => 10,
                'losecount' => 13
            ],
            [
                'id' => 7,  
                'wincount' => 110,
                'losecount' => 113
            ],
            [
                'id' => 8,  
                'wincount' => 80,
                'losecount' => 3
            ],
            [
                'id' => 9,  
                'wincount' => 100,
                'losecount' => 3
            ],
            [
                'id' => 10,  
                'wincount' => 100,
                'losecount' => 10
            ],
            [
                'id' => 11,  
                'wincount' => 100,
                'losecount' => 12
            ],
            [
                'id' => 12,  
                'wincount' => 100,
                'losecount' => 3
            ],
            [
                'id' => 13,  
                'wincount' => 100,
                'losecount' => 11
            ],
        ];

        // マスターデータを登録
        foreach ($playerstats as $playerstat) {
            \App\Models\PlayerStat::create($playerstat);
        }
    }
}