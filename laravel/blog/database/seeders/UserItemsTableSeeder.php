<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

class UserItemsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // ユーザーのアイテムのマスターデータ
        $userItems = [
            ['id' => 1, 'user_id' => 1, 'item_id' => 1, 'quantity' => 10],
            ['id' => 2, 'user_id' => 1, 'item_id' => 2, 'quantity' => 5],
            ['id' => 3, 'user_id' => 1, 'item_id' => 3, 'quantity' => 3],
            ['id' => 4, 'user_id' => 1, 'item_id' => 4, 'quantity' => 1],
            ['id' => 5, 'user_id' => 1, 'item_id' => 5, 'quantity' => 0],
            ['id' => 6, 'user_id' => 2, 'item_id' => 1, 'quantity' => 5],
            ['id' => 7, 'user_id' => 2, 'item_id' => 2, 'quantity' => 3],
            ['id' => 8, 'user_id' => 2, 'item_id' => 3, 'quantity' => 1],
            ['id' => 9, 'user_id' => 2, 'item_id' => 4, 'quantity' => 0],
            ['id' => 10, 'user_id' => 2, 'item_id' => 5, 'quantity' => 100],
            ['id' => 11, 'user_id' => 3, 'item_id' => 1, 'quantity' => 3],
            ['id' => 12, 'user_id' => 3, 'item_id' => 2, 'quantity' => 1],
            ['id' => 13, 'user_id' => 3, 'item_id' => 3, 'quantity' => 0],
            ['id' => 14, 'user_id' => 3, 'item_id' => 4, 'quantity' => 0],
            ['id' => 15, 'user_id' => 3, 'item_id' => 5, 'quantity' => 10],
        ];

        // マスターデータを登録
        foreach ($userItems as $userItem) {
            \App\Models\UserItem::create($userItem);
        }
    }
}
