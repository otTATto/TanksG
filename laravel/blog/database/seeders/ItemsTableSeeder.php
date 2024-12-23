<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

class ItemsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // アイテムのマスターデータ
        $items = [
            [
                // スタミナ回復アイテム
                'id' => 1, 
                'name' => 'Stamina Recovery Item', 
                'description' => 'スタミナを 1 回復しました。', 
                'type' => 'instant'
            ],     
            [
                // 装甲強化アイテム
                'id' => 2, 
                'name' => 'Defense Boost Item',
                'description' => '戦車の HP を 2 倍にしました。',
                'type' => 'duration'
            ],        
            [
                // 授業の単位
                'id' => 3, 
                'name' => 'Class Credit', 
                'description' => 'ゲーム開発学特論の授業の単位を 1 単位獲得しました。', 
                'type' => 'instant'
            ],              
            [
                // 経験値
                'id' => 4, 
                'name' => 'Experience Point',
                'description' => '人生の経験値を増やしました。',
                'type' => 'instant'
            ],          
            [
                // ゴールド
                'id' => 5, 
                'name' => 'Gold',
                'description' => 'お金を 1G 獲得しました。',
                'type' => 'instant'
            ],                      
        ];
        
        // マスターデータを登録
        foreach ($items as $item) {
            \App\Models\Item::create($item);
        }
    }
}
