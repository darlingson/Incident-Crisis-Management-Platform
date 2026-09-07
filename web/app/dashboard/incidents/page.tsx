"use client";

import React, { useState, useEffect } from "react";
import { 
  AlertCircle, 
  FolderOpen, 
  Clock, 
  UserMinus, 
  Search, 
  Filter, 
  Calendar, 
  Plus, 
  Download,
  MoreHorizontal,
  ChevronLeft,
  ChevronRight,
  Database,
  Globe,
  ShieldCheck
} from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";

// Severity/status use semantic tokens --severity-* / --status-* (single mapping)
const getSeverityStyle = (impact: string) => {
  const i = impact?.toLowerCase();
  if (i === "high" || i === "major") return "bg-severity-critical/10 text-severity-critical border-severity-critical/20";
  if (i === "medium" || i === "moderate") return "bg-severity-medium/10 text-severity-medium border-severity-medium/20";
  return "bg-severity-low/10 text-severity-low border-severity-low/20";
};

const getStatusStyle = (status: string) => {
  const s = status?.toLowerCase();
  if (s === "reported") return "bg-status-reported/10 text-status-reported border-status-reported/20";
  if (s === "underinvestigation" || s === "monitoring") return "bg-status-investigation/10 text-status-investigation border-status-investigation/20";
  if (s === "resolved" || s === "closed") return "bg-status-resolved/10 text-status-resolved border-status-resolved/20";
  return "bg-muted text-muted-foreground border-border";
};

export default function IncidentOverview() {
  const [loading, setLoading] = useState(true);
  const [reports, setReports] = useState<any[]>([]);

  useEffect(() => {
    fetchReports();
  }, []);

  const fetchReports = async () => {
    setLoading(true);
    try {
      const res = await fetch("/api/reports/");
      if (!res.ok) throw new Error("Failed to fetch reports");
      const data = await res.json();
      setReports(data);
    } catch (err: any) {
      toast.error(err.message || "Failed to load dashboard data");
    } finally {
      setLoading(false);
    }
  };

  // Stats calculation based on your data
  const criticalCount = reports.filter(r => r.impact?.toLowerCase() === 'high' || r.impact?.toLowerCase() === 'major').length;
  const unassignedCount = reports.filter(r => !r.assignedTo).length;

  return (
    <div className="min-h-screen bg-background text-foreground p-6 space-y-6 animate-in fade-in duration-300">
      {/* Header Section */}
      <div className="flex justify-between items-start">
        <div className="space-y-1">
          <h1 className="text-2xl font-semibold tracking-tight">Incident Overview</h1>
          <p className="text-muted-foreground text-sm">
            Real-time dashboard for managing active and past incidents.
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline">
            <Download className="w-4 h-4 mr-2" /> Export
          </Button>
          <Button>
            <Plus className="w-4 h-4 mr-2" /> Create Incident
          </Button>
        </div>
      </div>

      {/* Stats Grid — critical dominant */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {[
          { label: "Critical active", value: criticalCount, icon: AlertCircle, color: "text-destructive", prominent: true },
          { label: "Total open", value: reports.length, icon: FolderOpen, color: "text-primary" },
          { label: "Avg. ack time", value: "4m 30s", icon: Clock, color: "text-status-resolved" },
          { label: "Unassigned", value: unassignedCount, icon: UserMinus, color: "text-severity-medium" },
        ].map((stat, i) => (
          <Card key={i} className={`p-6 flex justify-between items-start ${stat.prominent ? "bg-destructive/5 border-l-4 border-l-severity-critical shadow-sm" : ""}`}>
            <CardContent className="p-0 space-y-2">
              <p className="text-xs text-muted-foreground">{stat.label}</p>
              <span className={`${stat.prominent ? "text-3xl font-semibold" : "text-2xl font-medium"}`}>{stat.value}</span>
            </CardContent>
            <stat.icon className={`w-4 h-4 ${stat.color}`} />
          </Card>
        ))}
      </div>

      {/* Filter Bar */}
      <Card className="p-4">
        <CardContent className="p-0 flex flex-wrap gap-3 items-center">
          <div className="relative flex-1 min-w-[300px]">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input 
              placeholder="Search by ID, title, affected service..." 
              className="pl-10 h-9"
            />
          </div>
          <Select defaultValue="all">
            <SelectTrigger className="w-[140px] h-9">
              <SelectValue placeholder="Severity" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Severity: All</SelectItem>
              <SelectItem value="high">Critical</SelectItem>
              <SelectItem value="low">Low</SelectItem>
            </SelectContent>
          </Select>
          <Select defaultValue="active">
            <SelectTrigger className="w-[140px] h-9">
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="active">Status: Active</SelectItem>
              <SelectItem value="resolved">Resolved</SelectItem>
            </SelectContent>
          </Select>
          <Button variant="outline" className="h-9">
            <Calendar className="w-4 h-4 mr-2" /> Date Range
          </Button>
          <Button variant="ghost" size="icon" className="border border-border"><Filter className="w-4 h-4" /></Button>
        </CardContent>
      </Card>

      {/* Table Container */}
      <Card className="overflow-hidden p-0">
        <Table>
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead>ID</TableHead>
              <TableHead>Incident Title</TableHead>
              <TableHead>Severity</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Owner</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {reports.map((report) => (
              <TableRow key={report.id} className="hover:bg-muted/50">
                <TableCell className="font-medium text-muted-foreground">#INC-{report.id}</TableCell>
                <TableCell>
                  <div className="space-y-0.5">
                    <p className="text-sm font-medium">{report.title}</p>
                    <p className="text-xs text-muted-foreground truncate max-w-[250px]">{report.narrative}</p>
                  </div>
                </TableCell>
                <TableCell>
                  <Badge className={`rounded-full px-3 py-0.5 border text-xs font-medium ${getSeverityStyle(report.impact)}`}>
                    {report.impact}
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge variant="outline" className={`px-2.5 py-0.5 text-xs ${getStatusStyle(report.status)}`}>
                    {report.status}
                  </Badge>
                </TableCell>
                <TableCell>
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <Globe className="w-4 h-4" />
                    <span className="text-xs font-medium">{report.location}</span>
                  </div>
                </TableCell>
                <TableCell>
                     <Avatar className="w-6 h-6 border border-border">
                       <AvatarFallback className="bg-muted text-xs font-medium">
                         {report.assignedTo ? report.assignedTo[0] : "?"}
                       </AvatarFallback>
                     </Avatar>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>

        {/* Footer / Pagination */}
        <div className="bg-muted/30 border-t border-border px-6 py-3 flex items-center justify-between text-xs text-muted-foreground">
          <p>Showing <span className="text-foreground font-medium">1 to {reports.length}</span> of <span className="text-foreground font-medium">{reports.length}</span> results</p>
          <div className="flex gap-2">
            <Button variant="outline" size="sm">Previous</Button>
            <Button variant="outline" size="sm">Next</Button>
          </div>
        </div>
      </Card>
    </div>
  );
}