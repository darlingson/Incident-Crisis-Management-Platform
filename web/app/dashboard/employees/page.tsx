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
import Link from 'next/link'

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

        <div className="flex flex-col space-y-1">
          <h1 className="text-3xl font-semibold tracking-tight">
            Good afternoon, <span className="text-primary">Alex</span>
          </h1>
          <p className="text-muted-foreground">
            CrisisCMD Employee Portal • Secure Incident Management
          </p>
        </div>
        <Card className="relative overflow-hidden bg-card border-border shadow-sm">
          <div className="absolute right-0 top-1/2 -translate-y-1/2 p-6 opacity-5 pointer-events-none">
            <Shield className="w-64 h-64 text-primary" />
          </div>

          <CardContent className="p-8 relative z-10">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">

              <div className="space-y-4">
                <Badge variant="outline" className="gap-2">
                  <span className="relative flex h-2 w-2">
                    <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-500 opacity-75"></span>
                    <span className="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
                  </span>
                  System Operational
                </Badge>
                <div>
                  <h2 className="text-2xl font-semibold tracking-tight mb-2">Report an Incident</h2>
                  <p className="text-muted-foreground text-sm max-w-md">
                    Spot a safety hazard or security breach? Notify the command center immediately to ensure site-wide safety.
                  </p>
                </div>
              </div>

              <div className="flex md:justify-end">
                <Link href="/dashboard/incidents/report">
                  <Button
                    size="lg"
                    className="px-8 h-12 group"
                  >
                    <PlusCircle className="w-5 h-5 mr-2 group-hover:rotate-90 transition-transform duration-300" />
                    Initiate Report
                  </Button>
                </Link>
              </div>

            </div>
          </CardContent>
        </Card>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

          <div className="lg:col-span-2 space-y-8">

            <div className="space-y-4">
              <div className="flex justify-between items-end px-1">
                <div className="space-y-1">
                  <h3 className="text-lg font-semibold tracking-tight flex items-center gap-2">
                    <Clock className="w-4 h-4 text-primary" />
                    Activity Log
                  </h3>
                  <p className="text-sm text-muted-foreground">Recent filings and status updates</p>
                </div>
                <Button variant="ghost" size="sm" className="text-primary">
                  View full history <ArrowRight className="ml-2 w-4 h-4" />
                </Button>
              </div>

              <ScrollArea className="h-[320px] rounded-md">
                <div className="space-y-3 pr-4">
                  <Card className="bg-card border-border hover:border-primary/20 transition-colors cursor-pointer group">
                    <CardContent className="p-5 flex items-center gap-5">
                      <div className="h-12 w-12 rounded-lg bg-muted flex items-center justify-center group-hover:bg-muted/80 transition-colors">
                        <Wifi className="w-5 h-5 text-severity-medium" />
                      </div>
                      <div className="flex-1 space-y-1">
                        <div className="flex justify-between">
                          <p className="font-medium">Wi-Fi Connectivity Issues on 3rd Floor</p>
                          <Badge variant="outline" className="bg-severity-medium/10 text-severity-medium border-severity-medium/20">Under Review</Badge>
                        </div>
                        <p className="text-xs text-muted-foreground font-mono">REF: #INC-599 • SUBMITTED: TODAY 16:30</p>
                      </div>
                    </CardContent>
                  </Card>

                  <Card className="bg-card border-border opacity-80 hover:opacity-100 transition-opacity">
                    <CardContent className="p-5 flex items-center gap-5">
                      <div className="h-12 w-12 rounded-lg bg-muted flex items-center justify-center">
                        <Package className="w-5 h-5 text-muted-foreground" />
                      </div>
                      <div className="flex-1 space-y-1">
                        <div className="flex justify-between">
                          <p className="font-medium">Access Credentials Lost</p>
                          <Badge variant="secondary">Closed</Badge>
                        </div>
                        <p className="text-xs text-muted-foreground font-mono">REF: #INC-380 • SUBMITTED: 24H AGO</p>
                      </div>
                    </CardContent>
                  </Card>
                </div>
              </ScrollArea>
            </div>
          </div>

          <div className="space-y-8">

            <Card className="bg-card border-border">
              <CardHeader className="pb-4">
                <CardTitle className="text-primary flex items-center gap-2 text-lg">
                  <HelpCircle className="w-4 h-4" /> Help & Support
                </CardTitle>
                <CardDescription>
                  Global response team available 24/7.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                <Tooltip>
                  <TooltipTrigger asChild>
                    <div className="flex items-center p-4 bg-card rounded-lg border border-border hover:border-primary/20 transition-colors cursor-help">
                      <Phone className="w-4 h-4 mr-4 text-primary" />
                      <div>
                        <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide leading-none mb-1">Emergency Line</p>
                        <p className="font-medium">EXT. 9110</p>
                      </div>
                    </div>
                  </TooltipTrigger>
                  <TooltipContent side="left">Internal Priority Line</TooltipContent>
                </Tooltip>

                <div className="flex items-center p-4 bg-card rounded-lg border border-border">
                  <Mail className="w-4 h-4 mr-4 text-muted-foreground" />
                  <div>
                    <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide leading-none mb-1">Global Helpdesk</p>
                    <p className="font-medium text-sm">support@crisiscmd.com</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            <div className="space-y-4 px-1">
              <h3 className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Compliance Resources</h3>
              <div className="grid gap-2">
                {[
                  { name: "Global Incident Policy", icon: FileText },
                  { name: "Safety & Security Protocols", icon: Shield },
                  { name: "Evacuation Route Maps", icon: Map },
                ].map((link) => (
                  <Button
                    key={link.name}
                    variant="ghost"
                    className="w-full justify-start text-muted-foreground hover:text-foreground hover:bg-muted h-10 px-3 group"
                  >
                    <link.icon className="w-4 h-4 mr-3 text-muted-foreground group-hover:text-primary transition-colors" />
                    {link.name}
                  </Button>
                ))}
              </div>
            </div>

            <Separator />

            <Card className="bg-primary/5 border-primary/10">
              <CardContent className="p-4">
                <p className="text-xs text-primary/80 leading-relaxed italic">
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