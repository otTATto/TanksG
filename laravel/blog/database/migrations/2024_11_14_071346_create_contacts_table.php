<?php
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateContactsTable extends Migration
{
    public function up()
    {
        Schema::create('contacts', function (Blueprint $table) {
            $table->id(); // 自動増分ID
            $table->unsignedBigInteger('user_id'); // 問い合わせをしたユーザーのID
            $table->string('title'); // お問い合わせのタイトル
            $table->text('content'); // お問い合わせの本文
            $table->timestamps(); // created_at と updated_at カラムを自動作成
        });
    }

    public function down()
    {
        Schema::dropIfExists('contacts');
    }
}
