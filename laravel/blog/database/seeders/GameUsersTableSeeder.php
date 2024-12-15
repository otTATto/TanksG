<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Str;

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
            [
                'id' => 1, 
                'uuid' => Str::uuid(),
                'name' => 'Alice', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 2, 
                'uuid' => Str::uuid(),
                'name' => 'Bob', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 3, 
                'uuid' => Str::uuid(),
                'name' => 'Charlie', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
        ];

        // マスターデータを登録
        foreach ($gameUsers as $gameUser) {
            \App\Models\GameUser::create($gameUser);
        }
    }
}
