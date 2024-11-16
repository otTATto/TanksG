<?php
namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\Contact;
use App\Models\Reply;
use Illuminate\Http\Request;

class ContactController extends Controller
{
    // すべてのお問い合わせを取得する
    public function index()
    {
        $contacts = Contact::orderBy('created_at', 'desc')->get();
        return response()->json($contacts);
    }

    // 新しいお問い合わせを作成する
    public function store(Request $request)
    {
        $request->validate([
            'user_id' => 'required|integer',
            'title' => 'required|string|max:255',
            'content' => 'required|string',
        ]);

        $contact = Contact::create([
            'user_id' => $request->user_id,
            'title' => $request->title,
            'content' => $request->content,
        ]);

        return response()->json($contact, 201);
    }

    // 特定のお問い合わせを取得する
    public function show($id)
    {
        $contact = Contact::with('replies')->findOrFail($id);
        return response()->json($contact);
    }

    // 特定のお問い合わせを更新する
    public function update(Request $request, $id)
    {
        $contact = Contact::findOrFail($id);

        $request->validate([
            'title' => 'sometimes|string|max:255',
            'content' => 'sometimes|string',
        ]);

        $contact->update($request->only(['title', 'content']));

        return response()->json($contact);
    }

    // 特定のお問い合わせを削除する
    public function destroy($id)
    {
        $contact = Contact::findOrFail($id);
        $contact->delete();

        return response()->json(null, 204);
    }

    // 返信を追加する
    public function addReply(Request $request, $contact_id)
    {
        // バリデーション
        $request->validate([
            'message' => 'required|string',
            'user_id' => 'required|integer',
        ]);

        // 返信を保存
        $reply = new Reply();
        $reply->contact_id = $contact_id; // どの問い合わせに対する返信か
        $reply->user_id = $request->user_id; // ユーザーID
        $reply->message = $request->message; // 返信内容
        $reply->save();

        // 返信が追加されたことを返す
        return response()->json(['message' => '返信が追加されました', 'reply' => $reply], 200);
    }
}
