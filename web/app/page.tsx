"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Shield, AlertCircle, Loader2, Users, Building, ClipboardList } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

export default function LoginPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [rememberMe, setRememberMe] = useState(true);

  async function onSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    setErrorMessage(null);

    const formData = new FormData(event.currentTarget);
    const email = formData.get("email");
    const password = formData.get("password");

    try {
      const response = await fetch("/api/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, allowUnconfirmedEmail: true }),
      });
      const data = await response.json();
      if (!response.ok) throw new Error(data.error || "Authentication failed. Please check your credentials.");
      router.refresh();
      router.push("/dashboard");
    } catch (err: any) {
      setErrorMessage(err.message);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="min-h-screen w-full flex flex-col md:flex-row font-sans text-foreground bg-white">
      {/* Left — Hero */}
      <div className="relative flex-1 bg-gradient-to-br from-slate-100 via-slate-50 to-blue-50/30 p-8 lg:p-12 flex flex-col justify-between overflow-hidden min-h-[500px] md:min-h-screen">
        <div className="max-w-md z-10 mt-6 md:mt-12 md:ml-8">
          <h1 className="text-3xl md:text-4xl lg:text-[42px] font-bold text-slate-900 leading-tight tracking-tight">
            Report, track and <br />
            resolve incidents — <br />
            <span className="text-slate-900">without the chaos.</span>
          </h1>
          <p className="text-sm text-muted-foreground mt-4 leading-relaxed">
            A minimal workspace for employees and managers to capture, triage and close incidents.
          </p>
        </div>

        <div className="relative w-full max-w-lg mx-auto h-[450px] my-auto flex items-center justify-center">
          <div className="absolute top-10 left-6 w-10 h-10 border-2 border-slate-300 rotate-12 opacity-40 polygon-hex" />
          <div className="absolute top-36 right-8 w-16 h-16 border-2 border-slate-200 -rotate-12 opacity-30 polygon-hex" />
          <div className="absolute bottom-8 left-1/3 w-12 h-12 border-2 border-slate-200 rotate-45 opacity-30 polygon-hex" />

          <div className="relative z-20 w-64 h-72 sm:w-80 sm:h-[340px] shadow-xl overflow-hidden polygon-hex bg-gradient-to-br from-primary/20 to-primary/5 border border-border flex items-center justify-center">
            <div className="text-center p-6">
              <div className="w-16 h-16 mx-auto mb-4 rounded-lg bg-primary flex items-center justify-center">
                <ClipboardList className="w-8 h-8 text-primary-foreground" />
              </div>
              <p className="text-sm font-medium">Centralized incident tracking</p>
              <p className="text-xs text-muted-foreground mt-1">All reports in one place</p>
            </div>
          </div>

          <div className="absolute top-6 right-10 sm:right-16 z-30 w-20 h-24 sm:w-24 sm:h-28 shadow-lg polygon-hex bg-primary flex items-center justify-center">
            <Users className="w-8 h-8 text-primary-foreground" />
          </div>

          <div className="absolute top-40 right-2 sm:right-6 z-30 w-16 h-20 sm:w-20 sm:h-24 shadow-md polygon-hex bg-slate-700 flex items-center justify-center">
            <Building className="w-6 h-6 text-white" />
          </div>

          <div className="absolute bottom-16 left-2 sm:left-8 z-30 w-20 h-24 sm:w-24 sm:h-28 shadow-lg polygon-hex bg-slate-900 flex items-center justify-center">
            <Shield className="w-8 h-8 text-white" />
          </div>

          <div className="absolute bottom-6 left-20 sm:left-28 z-30 w-14 h-16 sm:w-16 sm:h-20 shadow-md polygon-hex bg-muted border border-border flex items-center justify-center">
            <ClipboardList className="w-5 h-5 text-muted-foreground" />
          </div>
        </div>

        <div className="hidden md:block h-6" />
      </div>

      {/* Right — Form */}
      <div className="w-full md:w-[480px] lg:w-[540px] bg-white p-8 sm:p-12 lg:p-16 flex flex-col justify-between items-center">
        <div className="w-full flex justify-end mb-12">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg bg-primary flex items-center justify-center">
              <Shield className="w-4 h-4 text-primary-foreground" />
            </div>
            <span className="text-xl font-semibold tracking-tight">IncidentDesk</span>
          </div>
        </div>

        <div className="w-full max-w-sm my-auto">
          <h2 className="text-2xl font-bold text-foreground mb-8">Sign in</h2>
          <div className="mb-6">
            <p className="text-sm font-medium mb-1">Welcome back</p>
            <p className="text-xs text-muted-foreground leading-relaxed">
              Sign in to manage incidents and keep teams aligned.
            </p>
          </div>

          <form onSubmit={onSubmit} className="space-y-5">
            {errorMessage && (
              <Alert variant="destructive">
                <AlertCircle className="w-4 h-4" />
                <AlertTitle>Sign in failed</AlertTitle>
                <AlertDescription>{errorMessage}</AlertDescription>
              </Alert>
            )}

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1.5">Work email</label>
              <input
                type="email"
                name="email"
                placeholder="you@company.com"
                className="w-full px-3.5 py-2.5 text-sm rounded border border-input bg-background focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-muted-foreground/50 transition-colors"
                required
                disabled={isLoading}
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1.5">Password</label>
              <input
                type="password"
                name="password"
                placeholder="••••••••"
                className="w-full px-3.5 py-2.5 text-sm rounded border border-input bg-background focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-muted-foreground/50 transition-colors"
                required
                disabled={isLoading}
              />
            </div>

            <div className="flex items-center justify-between text-xs pt-1">
              <label className="flex items-center gap-2 cursor-pointer select-none">
                <input
                  type="checkbox"
                  checked={rememberMe}
                  onChange={(e) => setRememberMe(e.target.checked)}
                  className="w-4 h-4 rounded border-input text-primary focus:ring-primary accent-primary cursor-pointer"
                />
                <span className="text-muted-foreground">Remember me</span>
              </label>
              <Link href="#" className="text-xs text-primary hover:underline font-medium">
                Forgot password?
              </Link>
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="w-full py-3 bg-primary hover:bg-primary/90 text-primary-foreground text-xs tracking-wide font-semibold rounded shadow-sm transition-colors disabled:opacity-50 flex items-center justify-center gap-2"
            >
              {isLoading ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" /> Signing in...
                </>
              ) : (
                "SIGN IN"
              )}
            </button>
          </form>

          <div className="relative text-center my-6">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-border" />
            </div>
            <div className="relative flex justify-center text-xs">
              <span className="bg-white px-3 text-muted-foreground">Or continue with</span>
            </div>
          </div>

          <button
            type="button"
            className="w-full py-2.5 border border-input bg-background hover:bg-muted text-sm font-medium rounded flex items-center justify-center gap-2"
          >
            Single Sign-On (Azure AD)
          </button>

          <div className="mt-6 text-center text-xs text-muted-foreground">
            Protected system — contact admin for access.
          </div>
        </div>

        <div className="h-6" />
      </div>

      <style>{`.polygon-hex{clip-path:polygon(50% 0%,100% 25%,100% 75%,50% 100%,0% 75%,0% 25%)}`}</style>
    </div>
  );
}
