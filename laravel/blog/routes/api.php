<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\MyDataController;
use App\Http\Controllers\Api\ContactController;
use App\Http\Controllers\Api\GameUserController;
use App\Http\Controllers\Api\GiftController;


/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
    return $request->user();
});

Route::get('/blog', [MyDataController::class, 'getInfo']);
Route::apiResource('contact', ContactController::class);

// 返信を追加するエンドポイント
Route::post('contact/{contact_id}/replies', [ContactController::class, 'addReply']);

// ゲームユーザーのアカウント停止状態を確認するAPI
Route::get('game-users/{id}/check-suspended', [GameUserController::class, 'checkSuspended']);
Route::post('/grant-item', [GiftController::class, 'grantItem']);
Route::get('/presents/{user_id}', [GiftController::class, 'getUserPresents']); // プレゼント一覧
Route::post('/presents/received/{user_id}/{present_id}', [GiftController::class, 'receivePresentById']);
Route::post('/gameusers', [GameUserController::class, 'store']); // 新規ユーザー登録

// プレイヤー情報取得のルートを追加
Route::post('/getplayerinformation', [GameUserController::class, 'getPlayerInformation']);

// 名前変更のルートを修正
Route::post('gameusers/update-name', [GameUserController::class, 'updateName']);

// ランキング更新のルートを追加
Route::post('/updateranking', [GameUserController::class, 'updateRanking']);
