@extends('layouts.app')

@section('content')
<div class="container">
    <h1>アイテム管理</h1>

    <div class="accordion" id="accordionPanelsStayOpenExample">
        <div class="accordion-item">
            <div class="accordion-header">
                <button class="accordion-button fw-bold" type="button" data-bs-toggle="collapse" data-bs-target="#panelsStayOpen-collapseOne" aria-expanded="true" aria-controls="panelsStayOpen-collapseOne">
                    #1. アイテム追加とアイテム一覧
                </button>
            </div>
            <div id="panelsStayOpen-collapseOne" class="accordion-collapse collapse show">
                <div class="accordion-body">
                    <h3>アイテム追加</h3>
                    <form action="{{ route('admin.items.store') }}" method="POST">
                        @csrf
                        <div class="form-group">
                            <label for="item_name">Item Name</label>
                            <input type="text" class="form-control" id="item_name" name="item_name">
                        </div>
                        <button type="submit" class="btn btn-primary">追加</button>
                    </form>

                    <h3>アイテム一覧</h3>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Item ID</th>
                                <th>Item Name</th>
                                <th>Item Type</th>
                                <th>Item Descriptin</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach ($items as $item)
                                <tr>
                                    <td>{{ $item->id }}</td>
                                    <td>{{ $item->name }}</td>
                                    <td>{{ $item->type }}</td>
                                    <td>{{ $item->description }}</td>
                                </tr>
                            @endforeach
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="accordion-item">
            <div class="accordion-header">
                <button class="accordion-button collapsed fw-bold" type="button" data-bs-toggle="collapse" data-bs-target="#panelsStayOpen-collapseTwo" aria-expanded="false" aria-controls="panelsStayOpen-collapseTwo">
                #2. ユーザーごとのアイテム一覧
                </button>
            </div>
            <div id="panelsStayOpen-collapseTwo" class="accordion-collapse collapse">
                <div class="accordion-body">
                    <!-- ユーザー検索：当該ユーザーの所有アイテムのみを表示 -->
                    <h3>ユーザー検索</h3>
                    <form action="{{ route('admin.items.search') }}" method="GET">
                        @csrf
                        <div class="form-group">
                            <label for="user_id">User ID</label>
                            <input type="text" class="form-control" id="user_id" name="user_id">
                        </div>
                        <button type="submit" class="btn btn-primary">検索</button>
                    </form>
                    ※ 空欄検索によって全ユーザーを表示します

                    <h3>ユーザーごとのアイテム一覧</h3>
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
            </div>
        </div>
    </div>


    

</div>
@endsection