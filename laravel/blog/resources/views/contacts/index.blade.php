@extends('layouts.app')

@section('content')
<h1>お問い合わせ一覧</h1>

@foreach ($contacts as $contact)
    <h3>{{ $contact->title }}</h3>
    <p>{{ $contact->content }}</p>
    <small>お問い合わせ日時: {{ $contact->created_at }}</small>

    @foreach ($contact->replies as $reply)
        <p>{{ $reply->message }}</p>
        <small>{{ $reply->created_at }} ({{ $reply->user_id == auth()->id() ? '管理者' : 'ユーザー' }})</small>
    @endforeach

    <form action="{{ route('contacts.reply', $contact->id) }}" method="POST">
        @csrf
        <textarea name="message" rows="3" required></textarea>
        <button type="submit">返信する</button>
    </form>
@endforeach
@endsection
