"use client";

import React from "react";
import { 
  PlusCircle, 
  Clock, 
  HelpCircle, 
  Phone, 
  Mail, 
  FileText, 
  Shield, 
  Map,
  Wifi,
  Package,
  ArrowRight
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

export default function EmployeePortal() {
  return (
    <TooltipProvider>
      <div className="max-w-6xl mx-auto space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
        
        {/* Header Section */}
        <div className="flex flex-col space-y-1">
          <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl">
            Good afternoon, <span className="text-blue-500">Alex</span>
          </h1>
          <p className="text-muted-foreground text-lg">
            CrisisCMD Employee Portal • Secure Incident Management
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          {/* Left Column: Main Actions */}
          <div className="lg:col-span-2 space-y-8">
            
            {/* Hero Action Card */}
            <Card className="relative overflow-hidden border-zinc-800 bg-zinc-950/50 backdrop-blur-sm shadow-2xl">
              <div className="absolute top-0 right-0 p-6 opacity-10 pointer-events-none">
                <Shield className="w-48 h-48 rotate-12" />
              </div>
              
              <CardHeader className="relative z-10 pb-2">
                <Badge variant="outline" className="w-fit mb-4 bg-emerald-500/10 text-emerald-400 border-emerald-500/20 px-3 py-1">
                  <span className="relative flex h-2 w-2 mr-2">
                    <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
                    <span className="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
                  </span>
                  System Operational
                </Badge>
                <CardTitle className="text-4xl font-bold tracking-tight">Report an Incident</CardTitle>
                <CardDescription className="text-zinc-400 text-lg max-w-md pt-2">
                  Spot a safety hazard, IT issue, or security breach? Notify the command center immediately.
                </CardDescription>
              </CardHeader>

              <CardContent className="relative z-10 pt-6">
                <Button size="lg" className="bg-blue-600 hover:bg-blue-500 text-white rounded-xl px-10 h-14 text-lg font-bold shadow-lg shadow-blue-500/20 group">
                  <PlusCircle className="w-6 h-6 mr-3 group-hover:rotate-90 transition-transform duration-300" />
                  Initiate Report
                </Button>
              </CardContent>
            </Card>

            {/* My Reported Incidents */}
            <div className="space-y-4">
              <div className="flex justify-between items-end px-1">
                <div className="space-y-1">
                  <h3 className="text-xl font-bold tracking-tight flex items-center gap-2">
                    <Clock className="w-5 h-5 text-blue-500" />
                    Activity Log
                  </h3>
                  <p className="text-sm text-muted-foreground">Recent filings and status updates</p>
                </div>
                <Button variant="ghost" size="sm" className="text-blue-400 hover:text-blue-300 hover:bg-blue-500/10">
                  View full history <ArrowRight className="ml-2 w-4 h-4" />
                </Button>
              </div>

              <ScrollArea className="h-[320px] rounded-md">
                <div className="space-y-3 pr-4">
                  {/* Item 1 */}
                  <Card className="bg-zinc-900/40 border-zinc-800/50 hover:border-blue-500/50 transition-all duration-300 cursor-pointer group">
                    <CardContent className="p-5 flex items-center gap-5">
                      <div className="h-14 w-14 rounded-2xl bg-zinc-800 flex items-center justify-center group-hover:bg-zinc-700 transition-colors">
                        <Wifi className="w-7 h-7 text-orange-400" />
                      </div>
                      <div className="flex-1 space-y-1">
                        <div className="flex justify-between">
                          <p className="font-bold text-zinc-100">Wi-Fi Connectivity Issues on 3rd Floor</p>
                          <Badge className="bg-orange-500/10 text-orange-400 border-orange-500/20 font-medium">Under Review</Badge>
                        </div>
                        <p className="text-xs text-zinc-500 font-mono tracking-tighter">REF: #INC-599 • SUBMITTED: TODAY 16:30</p>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Item 2 */}
                  <Card className="bg-zinc-900/40 border-zinc-800/50 opacity-70 grayscale-[0.5] hover:grayscale-0 transition-all">
                    <CardContent className="p-5 flex items-center gap-5">
                      <div className="h-14 w-14 rounded-2xl bg-zinc-800 flex items-center justify-center">
                        <Package className="w-7 h-7 text-zinc-400" />
                      </div>
                      <div className="flex-1 space-y-1">
                        <div className="flex justify-between">
                          <p className="font-bold text-zinc-100">Access Credentials Lost</p>
                          <Badge variant="secondary" className="bg-zinc-800 text-zinc-500 border-zinc-700">Closed</Badge>
                        </div>
                        <p className="text-xs text-zinc-500 font-mono tracking-tighter">REF: #INC-380 • SUBMITTED: 24H AGO</p>
                      </div>
                    </CardContent>
                  </Card>
                </div>
              </ScrollArea>
            </div>
          </div>

          {/* Right Column: Widgets */}
          <div className="space-y-8">
            
            {/* Help & Support Widget */}
            <Card className="border-zinc-800 bg-zinc-900/20">
              <CardHeader className="pb-4">
                <CardTitle className="text-blue-500 flex items-center gap-2 text-lg">
                  <HelpCircle className="w-5 h-5" /> Help & Support
                </CardTitle>
                <CardDescription className="text-zinc-500">
                  Global response team available 24/7.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                <Tooltip>
                  <TooltipTrigger asChild>
                    <div className="flex items-center p-4 bg-zinc-950 rounded-xl border border-zinc-800/50 hover:border-zinc-700 transition-colors cursor-help">
                      <Phone className="w-5 h-5 mr-4 text-blue-500" />
                      <div>
                        <p className="text-[10px] text-zinc-600 uppercase font-bold tracking-widest leading-none mb-1">Emergency Line</p>
                        <p className="text-zinc-200 font-bold">EXT. 9110</p>
                      </div>
                    </div>
                  </TooltipTrigger>
                  <TooltipContent side="left">Internal Priority Line</TooltipContent>
                </Tooltip>

                <div className="flex items-center p-4 bg-zinc-950 rounded-xl border border-zinc-800/50">
                  <Mail className="w-5 h-5 mr-4 text-zinc-500" />
                  <div>
                    <p className="text-[10px] text-zinc-600 uppercase font-bold tracking-widest leading-none mb-1">Global Helpdesk</p>
                    <p className="text-zinc-200 font-bold text-sm">support@crisiscmd.com</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Useful Resources */}
            <div className="space-y-4 px-1">
              <h3 className="text-xs font-black text-zinc-600 uppercase tracking-[0.2em]">Compliance Resources</h3>
              <div className="grid gap-2">
                {[
                  { name: "Global Incident Policy", icon: FileText },
                  { name: "Safety & Security Protocols", icon: Shield },
                  { name: "Evacuation Route Maps", icon: Map },
                ].map((link) => (
                  <Button 
                    key={link.name} 
                    variant="ghost" 
                    className="w-full justify-start text-zinc-400 hover:text-white hover:bg-zinc-800/50 h-11 px-3 group"
                  >
                    <link.icon className="w-4 h-4 mr-3 text-zinc-700 group-hover:text-blue-500 transition-colors" />
                    {link.name}
                  </Button>
                ))}
              </div>
            </div>

            <Separator className="bg-zinc-800" />

            <Card className="bg-blue-600/5 border-blue-500/10">
              <CardContent className="p-4">
                <p className="text-xs text-blue-400/80 leading-relaxed italic">
                  "Ensuring a safe workplace is a collective responsibility. All reports remain confidential under the Global Safety Act."
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </TooltipProvider>
  );
}