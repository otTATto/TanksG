<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
    use App\Models\GiftLog;

class GiftLogController extends Controller
{
    public function index()
    {
        $giftLogs = GiftLog::with(['gameUser', 'item'])
            ->orderBy('created_at', 'desc')
            ->get();

        return view('admin.gift_logs.index', compact('giftLogs'));
    }
}
