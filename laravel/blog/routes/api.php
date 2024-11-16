<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\MyDataController;

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
use App\Http\Controllers\Api\ContactController;

Route::apiResource('contact', ContactController::class);


// 返信を追加するエンドポイント
Route::post('contact/{contact_id}/replies', [ContactController::class, 'addReply']);
