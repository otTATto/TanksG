@extends('layouts.app')

@section('content')
<div class="container">
    <h1>プレゼント配布履歴</h1>

    <table class="table table-bordered">
    <thead>
        <tr>
            <th>配布日時</th>
            <th>ユーザーID</th>
            <th>ユーザー名</th>
            <th>アイテムID</th>
            <th>アイテム名</th>
            <th>配布個数</th>
        </tr>
    </thead>
    <tbody>
        @foreach($giftLogs as $log)
            <tr>
                <td>{{ $log->created_at }}</td>
                <td>{{ $log->gameUser->id ?? '全体配布' }}</td>
                <td>{{ $log->gameUser->name ?? '全体配布' }}</td>
                <td>{{ $log->item->id }}</td>
                <td>{{ $log->item->name }}</td>
                <td>{{ $log->quantity }}</td>
            </tr>
        @endforeach
    </tbody>
</table>
</div>
@endsection
