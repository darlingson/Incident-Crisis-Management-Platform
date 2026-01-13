import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export async function PUT(
    request: NextRequest,
    { params }: { params: Promise<{ id: string }> }
) {
    try {
        const cookieStore = await cookies();
        const token = cookieStore.get('accessToken')?.value;
        const { id } = await params;
        const body = await request.json();
        console.log(body)

        if (!token) return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });

        const apiRes = await fetch(`${process.env.BASE_URL}/api/Reports/${id}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(body),
        });

        if (!apiRes.ok) {
            const errorText = await apiRes.text();
            return NextResponse.json({ error: errorText || 'Failed to update report' }, { status: apiRes.status });
        }

        return NextResponse.json({ success: true });
    } catch (error) {
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}