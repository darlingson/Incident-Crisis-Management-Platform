import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function proxy(request: NextRequest) {
  const token = request.cookies.get('accessToken')?.value;
  const userRole = request.cookies.get('userRole')?.value;

  console.log(`the role is ${userRole}`)

  if (!token && request.nextUrl.pathname.startsWith('/dashboard')) {
    return NextResponse.redirect(new URL('/login', request.url));
  }

  if (request.nextUrl.pathname === '/dashboard') {
    if (userRole === 'ComplianceOfficer') {
      return NextResponse.redirect(new URL('/dashboard/compliance', request.url));
    }

    return NextResponse.redirect(new URL('/dashboard/employees', request.url));
  }

  if (request.nextUrl.pathname.startsWith('/dashboard/compliance') && userRole !== 'ComplianceOfficer') {
    return NextResponse.redirect(new URL('/dashboard/employees', request.url));
  }

  return NextResponse.next();
}