import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

export async function POST(request: Request) {
    try {
        const cookieStore = await cookies();
        const token = cookieStore.get('accessToken')?.value;

        const formData = await request.formData();

        const apiRes = await fetch(`${process.env.BASE_URL}/api/Reports`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*'
            },
            body: formData,
        });

        if (!apiRes.ok) {
            const errorText = await apiRes.text();
            return NextResponse.json({ error: errorText || 'Failed to submit' }, { status: apiRes.status });
        }

        const data = await apiRes.json();
        return NextResponse.json(data);

    } catch (error) {
        console.error("Submission Error:", error);
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}