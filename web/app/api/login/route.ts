import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

export async function POST(request: Request) {
    try {
        const body = await request.json();

        const apiRes = await fetch(`${process.env.BASE_URL}/api/Auth/signin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': '*/*'
            },
            body: JSON.stringify(body),
        });

        if (!apiRes.ok) {
            return NextResponse.json({ error: 'Invalid credentials' }, { status: apiRes.status });
        }

        const data = await apiRes.json();

        const cookieStore = await cookies();

        cookieStore.set('accessToken', data.token, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            path: '/',
            maxAge: data.expiresIn * 60,
        });

        cookieStore.set('refreshToken', data.refreshToken, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            path: '/',
            maxAge: 60 * 60 * 24 * 7,
        });

        cookieStore.set('userRole', data.user.roles[0], {
            httpOnly: false,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'lax',
            path: '/',
        });

        cookieStore.set('userInfo', JSON.stringify({
            firstName: data.user.firstName,
            lastName: data.user.lastName,
            email: data.user.email
        }), {
            httpOnly: false,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'lax',
            path: '/',
        });

        return NextResponse.json({
            user: data.user,
            message: 'Login successful'
        });

    } catch (error) {
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}