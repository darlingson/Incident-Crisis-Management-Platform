"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Shield, Globe, Lock, AlertCircle, Loader2 } from "lucide-react";

export default function LoginPage() {
  const router = useRouter();

  // State for form handling
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

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
        body: JSON.stringify({
          email,
          password,
          allowUnconfirmedEmail: true 
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.error || "Authentication failed. Please check your credentials.");
      }

      router.refresh();
      router.push("/dashboard");

    } catch (err: any) {
      setErrorMessage(err.message);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div
      className="flex min-h-screen items-center justify-center bg-black text-white"
      style={{
        backgroundImage: `url(https://images5.alphacoders.com/353/thumb-1920-353018.jpg)`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundBlendMode: "overlay",
        backgroundColor: "rgba(0, 0, 0, 0.7)",
      }}
    >
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 max-w-7xl w-full px-8">

        {/* Left Side: Branding */}
        <div className="flex flex-col justify-center space-y-8">
          <div className="flex items-center space-x-4">
            <div className="p-3 bg-blue-600 rounded-lg shadow-lg shadow-blue-500/20">
              <Shield className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-4xl font-bold tracking-tight">CrisisCommand</h1>
          </div>

          <h2 className="text-5xl font-extrabold leading-tight tracking-tighter">
            Secure Operations & <br />
            <span className="text-blue-500">Incident Response</span>
          </h2>

          <p className="text-xl text-zinc-400 max-w-lg">
            Authenticate securely to access the unified command dashboard. Monitor critical infrastructure and coordinate response teams.
          </p>

          <div className="flex flex-wrap gap-4">
            <Badge variant="secondary" className="bg-emerald-900/30 text-emerald-400 border-emerald-800/50 px-4 py-2">
              <Globe className="w-4 h-4 mr-2" />
              System Operational
            </Badge>
            <Badge variant="secondary" className="bg-blue-900/30 text-blue-400 border-blue-800/50 px-4 py-2">
              <Lock className="w-4 h-4 mr-2" />
              End-to-End Encrypted
            </Badge>
          </div>
        </div>

        {/* Right Side: Login Card */}
        <div className="flex items-center justify-center">
          <Card className="w-full max-w-md bg-zinc-950/50 border-zinc-800 backdrop-blur-xl shadow-2xl">
            <CardHeader className="space-y-1 text-center">
              <div className="flex justify-center mb-6 space-x-2">
                <Button variant="outline" size="sm" className="text-xs border-zinc-700 bg-zinc-900/50 text-blue-400">
                  Incident Manager
                </Button>
                <Button variant="ghost" size="sm" className="text-xs text-zinc-500 hover:text-zinc-300">
                  Reporter
                </Button>
                <Button variant="ghost" size="sm" className="text-xs text-zinc-500 hover:text-zinc-300">
                  Executive
                </Button>
              </div>
              <CardTitle className="text-2xl font-bold">Commander Login</CardTitle>
              <CardDescription className="text-zinc-500">
                Enter your secure credentials to proceed
              </CardDescription>
            </CardHeader>

            <form onSubmit={onSubmit}>
              <CardContent className="space-y-4">

                {/* Error Alert Box */}
                {errorMessage && (
                  <div className="flex items-center gap-3 p-3 text-sm border rounded-lg bg-red-950/20 border-red-900/50 text-red-400 animate-in fade-in zoom-in duration-200">
                    <AlertCircle className="w-4 h-4 shrink-0" />
                    <p>{errorMessage}</p>
                  </div>
                )}

                <div className="space-y-2">
                  <Label htmlFor="email" className="text-zinc-400">Work Email</Label>
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    placeholder="commander@crisiscommand.com"
                    required
                    disabled={isLoading}
                    className="bg-zinc-900/50 border-zinc-800 focus:ring-blue-500 transition-all"
                  />
                </div>

                <div className="space-y-2">
                  <div className="flex justify-between items-center">
                    <Label htmlFor="password" className="text-zinc-400">Master Password</Label>
                    <button type="button" className="text-xs text-blue-500 hover:text-blue-400 hover:underline">
                      Recovery Protocol?
                    </button>
                  </div>
                  <Input
                    id="password"
                    name="password"
                    type="password"
                    required
                    disabled={isLoading}
                    className="bg-zinc-900/50 border-zinc-800 focus:ring-blue-500 transition-all"
                  />
                </div>

                <Button
                  type="submit"
                  disabled={isLoading}
                  className="w-full bg-blue-600 hover:bg-blue-500 text-white font-semibold py-6 transition-all"
                >
                  {isLoading ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Authenticating...
                    </>
                  ) : (
                    "Authorize Dashboard Access →"
                  )}
                </Button>

                <div className="relative text-center text-[10px] text-zinc-600 my-4 uppercase tracking-widest">
                  <span className="relative z-10 bg-zinc-950 px-3">Secure SSO Gateway</span>
                  <div className="absolute inset-x-0 top-1/2 h-[1px] bg-zinc-800" />
                </div>

                <Button variant="outline" type="button" className="w-full border-zinc-800 bg-zinc-900/30 hover:bg-zinc-800 text-zinc-400">
                  Single Sign-On (Azure AD)
                </Button>
              </CardContent>
            </form>

            <CardFooter className="pt-0">
              <div className="w-full bg-red-950/30 border border-red-900/20 text-red-500/80 text-[10px] font-bold text-center py-2 rounded uppercase tracking-tighter">
                ⚠️ Restricted Access: Unauthorized Entry Logged
              </div>
            </CardFooter>
          </Card>
        </div>
      </div>
    </div>
  );
}