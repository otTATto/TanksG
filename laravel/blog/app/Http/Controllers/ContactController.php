<?php

namespace App\Http\Controllers;

use App\Models\Contact;
use App\Models\Reply;
use Illuminate\Http\Request;

class ContactController extends Controller
{
    // 全てのお問い合わせを取得する
    public function index()
    {
        $contacts = Contact::with('replies')->orderBy('created_at', 'desc')->get();
        return view('contacts.index', compact('contacts'));
    }

    // お問い合わせに対する返信を保存
    public function storeReply(Request $request, $contactId)
    {
        $request->validate([
            'message' => 'required|string',
        ]);

        $reply = Reply::create([
            'contact_id' => $contactId,
            'user_id' => auth()->id(), // 管理者のID
            'message' => $request->message,
        ]);

        return redirect()->back()->with('success', '返信が送信されました。');
    }
    // app/Http/Controllers/ContactController.php

    // 既存のメソッド

    public function reply(Request $request, Contact $contact)
    {
        // バリデーション
        $request->validate([
            'message' => 'required|string',
        ]);

        // 返信を保存
        $reply = new Reply();
        $reply->message = $request->message;
        $reply->contact_id = $contact->id;
        $reply->user_id = auth()->id(); // 管理者IDを保存
        $reply->save();

        // お問い合わせ詳細ページにリダイレクト
        return redirect()->route('contacts.index')->with('success', '返信が送信されました');
    }
}
