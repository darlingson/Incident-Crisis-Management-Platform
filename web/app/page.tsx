import Image from "next/image";
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
import { Shield, Globe, Lock } from "lucide-react";

export default function Home() {
  return (
    <div
      className="flex min-h-screen items-center justify-center bg-black text-white"
      style={{
        backgroundImage: `ur[](https://images5.alphacoders.com/353/thumb-1920-353018.jpg)`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundBlendMode: "overlay",
        backgroundColor: "rgba(0, 0, 0, 0.6)",
      }}
    >
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 max-w-7xl w-full px-8">
        {/* Left Side: Branding */}
        <div className="flex flex-col justify-center space-y-8">
          <div className="flex items-center space-x-4">
            <div className="p-3 bg-blue-600 rounded-lg">
              <Shield className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-4xl font-bold">CrisisCommand</h1>
          </div>

          <h2 className="text-5xl font-bold leading-tight">
            Secure Operations & Incident Response
          </h2>

          <p className="text-xl text-zinc-300">
            Authenticate securely to access the unified command dashboard. Monitor critical infrastructure, manage incidents, and coordinate response teams in real-time.
          </p>

          <div className="flex gap-4">
            <Badge variant="secondary" className="bg-green-900 text-green-100 px-4 py-2">
              <Globe className="w-4 h-4 mr-2" />
              System Operational
            </Badge>
            <Badge variant="secondary" className="bg-blue-900 text-blue-100 px-4 py-2">
              <Lock className="w-4 h-4 mr-2" />
              Connection End-to-End Encrypted
            </Badge>
          </div>
        </div>

        {/* Right Side: Login Card */}
        <div className="flex items-center justify-center">
          <Card className="w-full max-w-md bg-zinc-900/80 border-zinc-800 backdrop-blur">
            <CardHeader className="space-y-1 text-center">
              <div className="flex justify-center mb-4">
                <Button variant="ghost" size="sm" className="text-blue-400">
                  Incident Manager
                </Button>
                <Button variant="ghost" size="sm">
                  Reporter
                </Button>
                <Button variant="ghost" size="sm">
                  Executive
                </Button>
              </div>
              <CardTitle className="text-2xl">Welcome back</CardTitle>
              <CardDescription className="text-zinc-400">
                Please enter your credentials to access the Incident Manager dashboard.
              </CardDescription>
            </CardHeader>

            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Work Email</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="name@enterprise.com"
                  className="bg-zinc-800 border-zinc-700"
                  defaultValue="name@enterprise.com"
                />
              </div>

              <div className="space-y-2">
                <div className="flex justify-between">
                  <Label htmlFor="password">Password</Label>
                  <a href="#" className="text-sm text-blue-400 hover:underline">
                    Forgot password?
                  </a>
                </div>
                <Input
                  id="password"
                  type="password"
                  className="bg-zinc-800 border-zinc-700"
                />
              </div>

              <Button className="w-full bg-blue-600 hover:bg-blue-700">
                Sign In to Dashboard →
              </Button>

              <div className="relative text-center text-sm text-zinc-500 my-4">
                <span className="bg-zinc-900/80 px-2">OR CONTINUE WITH</span>
                <div className="absolute inset-x-0 top-1/2 h-px bg-zinc-700" />
              </div>

              <Button variant="outline" className="w-full border-zinc-700">
                Single Sign-On (SSO)
              </Button>
            </CardContent>

            <CardFooter>
              <div className="w-full bg-red-900/50 text-red-100 text-center py-3 rounded-b-lg">
                ⚠️ EMERGENCY READ-ONLY ACCESS
              </div>
            </CardFooter>
          </Card>
        </div>
      </div>
    </div>
  );
}