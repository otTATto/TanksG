<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\ArticleController;
use App\Http\Controllers\HomeController;
use App\Http\Controllers\ContactController;
use App\Http\Controllers\Admin\GameUserController;
use App\Http\Controllers\Admin\GiftLogController;

// 一般的なルート設定
Route::get('/', function () {
    return view('welcome');
});

// お知らせ関連のルート
Route::get('/articles', [ArticleController::class, 'index'])->name('articles.index');
Route::get('/articles/create', [ArticleController::class, 'create'])->name('articles.create');
Route::post('/articles', [ArticleController::class, 'store'])->name('articles.store');
Route::get('/articles/{article}', [ArticleController::class, 'show'])->name('articles.show');
Route::get('/articles/{article}/edit', [ArticleController::class, 'edit'])->name('articles.edit');
Route::patch('/articles/{article}', [ArticleController::class, 'update'])->name('articles.update');
Route::delete('/articles/{article}', [ArticleController::class, 'destroy'])->name('articles.destroy');

// お問い合わせ関連のルート
Route::get('/contacts', [ContactController::class, 'index'])->name('contacts.index');
Route::post('/contacts/{contact}/reply', [ContactController::class, 'reply'])->name('contacts.reply');
Route::get('/contact', [ContactController::class, 'index'])->name('contact.index');

// 認証されたユーザー用のルート
Route::group(['middleware' => ['auth']], function () {
    Route::get('/home', [HomeController::class, 'index'])->name('home');
    Route::resource('/articles', ArticleController::class);
});

// 管理者用ルート（管理者のみアクセス可能）
Route::prefix('admin')->middleware(['auth', 'admin'])->name('admin.')->group(function () {
    // ゲームユーザー管理
    Route::get('game-users', [GameUserController::class, 'index'])->name('game_users.index');
    Route::resource('game_users', GameUserController::class);
    // アカウント停止・復活
    Route::post('game_users/{game_user}/toggle-suspend', [GameUserController::class, 'toggleSuspend'])->name('game_users.toggleSuspend');

    // プレゼント配布履歴
    Route::get('/gift-logs', [GiftLogController::class, 'index'])->name('gift_logs.index');
});