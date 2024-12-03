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
            ['id' => 1, 'name' => 'Stamina Recovery Item'],     // スタミナ回復アイテム
            ['id' => 2, 'name' => 'Defense Boost Item'],        // 装甲強化アイテム
            ['id' => 3, 'name' => 'Class Credit'],              // 授業の単位
            ['id' => 4, 'name' => 'Experience Point'],          // 経験値
            ['id' => 5, 'name' => 'Gold'],                      // ゴールド
        ];
        
        // マスターデータを登録
        foreach ($items as $item) {
            \App\Models\Item::create($item);
        }
    }
}
