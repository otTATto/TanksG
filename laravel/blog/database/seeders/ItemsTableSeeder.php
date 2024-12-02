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
            ['id' => 1, 'name' => 'スタミナ回復アイテム'],
            ['id' => 2, 'name' => '装甲強化アイテム'],
            ['id' => 3, 'name' => '授業の単位'],
        ];
        
        // マスターデータを登録
        foreach ($items as $item) {
            \App\Models\Item::create($item);
        }
    }
}
