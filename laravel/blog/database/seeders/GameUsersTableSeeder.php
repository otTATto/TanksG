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
            [
                'id' => 4,
                'uuid' => Str::uuid(),
                'name' => '4', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 5, 
                'uuid' => Str::uuid(),
                'name' => '5', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 6, 
                'uuid' => Str::uuid(),
                'name' => '6', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 7, 
                'uuid' => Str::uuid(),
                'name' => '7', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 8, 
                'uuid' => Str::uuid(),
                'name' => '8', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 9, 
                'uuid' => Str::uuid(),
                'name' => '9', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 10, 
                'uuid' => Str::uuid(),
                'name' => '10', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 11, 
                'uuid' => Str::uuid(),
                'name' => '11', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 12, 
                'uuid' => Str::uuid(),
                'name' => '12', 
                'is_suspended' => 0,
                'stamina' => 3
            ],
            [
                'id' => 13, 
                'uuid' => Str::uuid(),
                'name' => '13', 
                'is_suspended' => 0,
                'stamina' => 3
            ]
        ];

        // マスターデータを登録
        foreach ($gameUsers as $gameUser) {
            \App\Models\GameUser::create($gameUser);
        }
    }
}
