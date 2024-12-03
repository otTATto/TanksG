@extends('layouts.app')

@section('content')
<div class="container">
    <h1>アイテム管理</h1>

    <!-- ユーザー検索：当該ユーザーの所有アイテムのみを表示 -->
    <form action="{{ route('admin.items.search') }}" method="GET">
        @csrf
        <div class="form-group">
            <label for="user_id">User ID</label>
            <input type="text" class="form-control" id="user_id" name="user_id">
        </div>
        <button type="submit" class="btn btn-primary">検索</button>
    </form>

    <h2>アイテム一覧</h2>

    <table class="table">
        <thead>
            <tr>
                <th>User ID</th>
                <th>User Name</th>
                <th>Item ID</th>
                <th>Item Name</th>
                <th>Item quantity</th>
            </tr>
        </thead>
        <tbody>
            @foreach ($userItems as $userItem)
                <tr>
                    <td>{{ $userItem->user_id }}</td>
                    <td>
                        @foreach ($gameUsers as $gameUser)
                            @if ($gameUser->id == $userItem->user_id)
                                {{ $gameUser->name }}
                            @endif
                        @endforeach
                    </td>
                    <td>{{ $userItem->item_id }}</td>
                    <td>
                        @foreach ($items as $item)
                            @if ($item->id == $userItem->item_id)
                                {{ $item->name }}
                            @endif
                        @endforeach
                    </td>
                    <td>{{ $userItem->quantity }}</td>
                </tr>
            @endforeach
        </tbody>
    </table>

</div>
@endsection