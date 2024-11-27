<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up()
    {
        Schema::create('game_users', function (Blueprint $table) {
            $table->id();
            $table->uuid('uuid')->unique();
            $table->string('name')->default('NoName');
            $table->integer('ranking')->default(0);
            $table->integer('winrate')->default(0);
            $table->integer('wincount')->default(0);
            $table->integer('losecount')->default(0);
            $table->boolean('is_suspended')->default(false);
            $table->timestamps();
        });
    }

    public function down()
    {
        Schema::dropIfExists('game_users');
    }
};
