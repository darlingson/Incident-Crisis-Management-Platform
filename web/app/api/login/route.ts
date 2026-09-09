import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';
import { logger } from '@/lib/logger';

export async function POST(request: Request) {
    try {
        const body = await request.json();
        logger.info("login attempt", { email: body.email });

        const baseUrl = process.env.BASE_URL;
        if (!baseUrl) {
            logger.error("BASE_URL not configured");
            return NextResponse.json({ error: 'Server misconfigured' }, { status: 500 });
        }

        const apiRes = await fetch(`${baseUrl}/api/Auth/signin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': '*/*'
            },
            body: JSON.stringify(body),
        });

        const data = await apiRes.json().catch(() => null);

        if (!apiRes.ok) {
            const msg = data?.message || data?.error || 'Invalid credentials';
            logger.warn("login failed", { email: body.email, status: apiRes.status, message: msg });
            return NextResponse.json({ error: msg }, { status: apiRes.status });
        }

        if (!data?.token || !data?.user) {
            logger.error("login succeeded but response malformed", { email: body.email, hasToken: !!data?.token, hasUser: !!data?.user });
            return NextResponse.json({ error: 'Invalid credentials' }, { status: 500 });
        }

        const roles: string[] = Array.isArray(data.user.roles) ? data.user.roles : Array.isArray(data.roles) ? data.roles : [];
        const role = roles[0] ?? null;
        if (!role) {
            logger.warn("login succeeded but role missing", { email: body.email, roles });
        }

        const cookieStore = await cookies();

        cookieStore.set('accessToken', data.token, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            path: '/',
            maxAge: (data.expiresIn ?? 60) * 60,
        });

        cookieStore.set('refreshToken', data.refreshToken, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            path: '/',
            maxAge: 60 * 60 * 24 * 7,
        });

        if (role) {
            cookieStore.set('userRole', role, {
                httpOnly: false,
                secure: process.env.NODE_ENV === 'production',
                sameSite: 'lax',
                path: '/',
            });
        }

        cookieStore.set('userInfo', JSON.stringify({
            firstName: data.user.firstName ?? data.user.firstName ?? "",
            lastName: data.user.lastName ?? "",
            email: data.user.email ?? body.email
        }), {
            httpOnly: false,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'lax',
            path: '/',
        });

        logger.info("login succeeded", { email: body.email, role });
        return NextResponse.json({
            user: data.user,
            message: 'Login successful'
        });

    } catch (error) {
        logger.error("login exception", { error: error instanceof Error ? error.message : String(error), stack: error instanceof Error ? error.stack : undefined });
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}