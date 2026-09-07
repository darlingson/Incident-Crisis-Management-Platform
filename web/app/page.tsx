"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Shield, AlertCircle, Loader2 } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

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
    <div className="flex min-h-screen items-center justify-center bg-background text-foreground">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 max-w-6xl w-full px-6 py-8">

        {/* Left Side: Branding */}
        <div className="flex flex-col justify-center space-y-6 bg-muted/30 rounded-lg p-8 border border-border">
          <div className="flex items-center gap-3">
            <div className="p-2.5 bg-primary rounded-lg">
              <Shield className="w-5 h-5 text-primary-foreground" />
            </div>
            <h1 className="text-2xl font-semibold tracking-tight">IncidentDesk</h1>
          </div>

          <h2 className="text-3xl font-semibold leading-tight tracking-tight">
            Report, track, and resolve incidents across your organization.
          </h2>

          <p className="text-sm text-muted-foreground max-w-md leading-relaxed">
            Sign in to manage incidents, track response, and keep teams aligned.
          </p>
        </div>

        {/* Right Side: Login Card */}
        <div className="flex items-center justify-center">
          <Card className="w-full max-w-md bg-card border-border shadow-sm">
            <CardHeader className="space-y-1 text-center">
              <div className="flex justify-center mb-6 gap-2">
                <Button variant="default" size="sm" className="text-xs">
                  Incident Manager
                </Button>
                <Button variant="ghost" size="sm" className="text-xs">
                  Reporter
                </Button>
                <Button variant="ghost" size="sm" className="text-xs">
                  Executive
                </Button>
              </div>
              <CardTitle className="text-xl font-semibold">Sign in</CardTitle>
              <CardDescription>
                Enter your credentials to continue
              </CardDescription>
            </CardHeader>

            <form onSubmit={onSubmit}>
              <CardContent className="space-y-4">

                {/* Error Alert Box */}
                {errorMessage && (
                  <Alert variant="destructive" className="animate-in fade-in zoom-in duration-200">
                    <AlertCircle className="w-4 h-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>{errorMessage}</AlertDescription>
                  </Alert>
                )}

                <div className="space-y-2">
                  <Label htmlFor="email">Work Email</Label>
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    placeholder="you@company.com"
                    required
                    disabled={isLoading}
                  />
                </div>

                <div className="space-y-2">
                  <div className="flex justify-between items-center">
                    <Label htmlFor="password">Password</Label>
                    <button type="button" className="text-xs text-primary hover:underline">
                      Forgot password?
                    </button>
                  </div>
                  <Input
                    id="password"
                    name="password"
                    type="password"
                    required
                    disabled={isLoading}
                  />
                </div>

                <Button
                  type="submit"
                  disabled={isLoading}
                  className="w-full"
                >
                  {isLoading ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Signing in...
                    </>
                  ) : (
                    "Sign in"
                  )}
                </Button>

                <div className="relative text-center text-xs text-muted-foreground my-4">
                  <span className="relative z-10 bg-card px-3">Or continue with</span>
                  <div className="absolute inset-x-0 top-1/2 h-[1px] bg-border" />
                </div>

                <Button variant="outline" type="button" className="w-full">
                  Single Sign-On (Azure AD)
                </Button>
              </CardContent>
            </form>


          </Card>
        </div>
      </div>
    </div>
  );
}