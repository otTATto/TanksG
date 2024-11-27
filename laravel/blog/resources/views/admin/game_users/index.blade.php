@extends('layouts.app')

@section('content')
    <h1>Game User List</h1>

    @if (session('status'))
        <div class="alert alert-success">
            {{ session('status') }}
        </div>
    @endif

    <table class="table">
        <thead>
            <tr>
                <th>User ID</th>
                <th>User Name</th>
                <th>Account Status</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach ($gameUsers as $gameUser)
                <tr>
                    <td>{{ $gameUser->id }}</td>
                    <td>{{ $gameUser->name }}</td>
                    <td>{{ $gameUser->is_suspended ? 'Suspended' : 'Active' }}</td>
                    <td>
                        <form action="{{ route('admin.game_users.toggleSuspend', $gameUser->id) }}" method="POST">
                            @csrf
                            @method('POST')
                            <button type="submit" class="btn btn-{{ $gameUser->is_suspended ? 'success' : 'danger' }}">
                                {{ $gameUser->is_suspended ? 'Restore' : 'Suspend' }}
                            </button>
                        </form>
                    </td>
                </tr>
            @endforeach
        </tbody>
    </table>
@endsection
