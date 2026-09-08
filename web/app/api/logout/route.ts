import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';
import { logger } from '@/lib/logger';

export async function POST(request: Request) {
  try {
    const cookieStore = await cookies();
    const token = cookieStore.get('accessToken')?.value;

    // Forward to Api to invalidate refresh token (best effort)
    if (token) {
      const baseUrl = process.env.BASE_URL;
      if (baseUrl) {
        try {
          await fetch(`${baseUrl}/api/Auth/logout`, {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json',
            },
          }).catch(() => null);
          logger.info("logout forwarded to Api", { hasToken: !!token });
        } catch (e) {
          logger.warn("logout forward failed", { error: e instanceof Error ? e.message : String(e) });
        }
      }
    }

    // Clear cookies
    const cookiesToClear = ['accessToken', 'refreshToken', 'userRole', 'userInfo'] as const;
    for (const name of cookiesToClear) {
      cookieStore.set(name, '', { path: '/', maxAge: 0 });
    }
    logger.info("logout cookies cleared");

    const accept = request.headers.get('accept') || '';
    // Form POST from Sidebar has text/html accept -> redirect, fetch JSON -> json
    if (accept.includes('text/html')) {
      return NextResponse.redirect(new URL('/', request.url));
    }
    return NextResponse.json({ message: 'Logged out' });

  } catch (error) {
    logger.error("logout exception", { error: error instanceof Error ? error.message : String(error) });
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
  }
}

// Also handle GET for direct navigation
export async function GET(request: Request) {
  return POST(request);
}
