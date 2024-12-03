<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

class GameUsersTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // ゲームユーザーのマスターデータ
        $gameUsers = [
            ['id' => 1, 'name' => 'Alice', 'is_suspended' => 0],
            ['id' => 2, 'name' => 'Bob', 'is_suspended' => 0],
            ['id' => 3, 'name' => 'Charlie', 'is_suspended' => 0],
        ];

        // マスターデータを登録
        foreach ($gameUsers as $gameUser) {
            \App\Models\GameUser::create($gameUser);
        }
    }
}
