<?php
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateRepliesTable extends Migration
{
    public function up()
    {
        Schema::create('replies', function (Blueprint $table) {
            $table->id(); // 自動増分ID
            $table->unsignedBigInteger('contact_id'); // contacts テーブルとのリレーション
            $table->unsignedBigInteger('user_id'); // 返信者のID（管理者またはユーザー）
            $table->text('message'); // 返信内容
            $table->timestamps(); // created_at と updated_at カラムを自動作成
        });
    }

    public function down()
    {
        Schema::dropIfExists('replies');
    }
}