<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateLoginBonusMasterTable extends Migration
{
    public function up()
    {
        Schema::create('login_bonus_master', function (Blueprint $table) {
            $table->id();
            $table->integer('day')->comment('1~7日目など');
            $table->unsignedBigInteger('item_id');
            $table->integer('quantity')->default(1);
            $table->timestamps();

            $table->foreign('item_id')->references('id')->on('items')->onDelete('cascade');
        });

        // ここでマスターデータを投入するシーディングを行うのも良い
        // 例: Day1~7までそれぞれitem_id=1~5など任意のアイテムを割り当てる
    }

    public function down()
    {
        Schema::dropIfExists('login_bonus_master');
    }
}
