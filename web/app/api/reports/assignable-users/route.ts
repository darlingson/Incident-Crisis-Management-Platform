import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

export async function GET(request: Request) {
    try {
        const cookieStore = await cookies();
        const token = cookieStore.get('accessToken')?.value;
        if (!token) {
            return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
        }
        const apiRes = await fetch(`${process.env.BASE_URL}/api/Reports/assignable-users`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*'
            }
        });

        if (!apiRes.ok) {
            const errorText = await apiRes.text();
            console.log(errorText)
            return NextResponse.json({ error: errorText || 'Failed to get users' }, { status: apiRes.status });
        }

        const data = await apiRes.json();
        console.log(data)
        return NextResponse.json(data);
    } catch (error) {
        console.error("Get error:", error);
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}