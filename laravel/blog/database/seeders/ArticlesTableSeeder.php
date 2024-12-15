<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

class ArticlesTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        // お知らせのマスターデータ
        $articles = [
            ['id' => 1, 'title' => 'Notification Test', 'body' => 'This is a test of the notification.', 'user_id' => 1],
            ['id' => 2, 'title' => 'Hello', 'body' => 'How are you? It’s a nice day today.', 'user_id' => 1],
            ['id' => 3, 'title' => 'Sorry', 'body' => 'For now, I have apologized. I’m sorry. It wasn’t a mistake but an apology. I made a mistake.', 'user_id' => 2],
        ];

        // マスターデータを登録
        foreach ($articles as $article) {
            \App\Models\Article::create($article);
        }
    }
}
