<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class UsersTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // ユーザー（管理者）のマスターデータ
        $users = [
            ['id' => 1, 'name' => 'Admin', 'email' => 'admin@example.com', 'is_admin' => true, 'password' => Hash::make('adminadmin')],
            ['id' => 2, 'name' => 'Taro', 'email' => 'taro@example.com', 'is_admin' => true, 'password' => Hash::make('tarotaro')],
            ['id' => 3, 'name' => 'Akuma', 'email' => 'akuma@example.com', 'is_admin' => false, 'password' => Hash::make('akumadayo')],
        ];

        // マスターデータを登録
        foreach ($users as $user) {
            \App\Models\User::create($user);
        }
    }
}
