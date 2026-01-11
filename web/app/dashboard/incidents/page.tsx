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
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";

// Helper to determine severity badge styles based on your data's "impact"
const getSeverityStyle = (impact: string) => {
  const i = impact?.toLowerCase();
  if (i === "high" || i === "major") return "bg-red-500/10 text-red-500 border-red-500/20";
  if (i === "medium" || i === "moderate") return "bg-orange-500/10 text-orange-500 border-orange-500/20";
  return "bg-zinc-500/10 text-zinc-400 border-zinc-500/20";
};

// Helper to determine status badge styles based on your data's "status"
const getStatusStyle = (status: string) => {
  const s = status?.toLowerCase();
  if (s === "reported") return "bg-blue-500/10 text-blue-400 border-blue-500/20";
  if (s === "underinvestigation" || s === "monitoring") return "bg-purple-500/10 text-purple-400 border-purple-500/20";
  if (s === "resolved" || s === "closed") return "bg-emerald-500/10 text-emerald-400 border-emerald-500/20";
  return "bg-zinc-500/10 text-zinc-400 border-zinc-500/20";
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
    <div className="min-h-screen bg-[#09090b] text-zinc-100 p-8 space-y-8 animate-in fade-in duration-500">
      {/* Header Section */}
      <div className="flex justify-between items-start">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Incident Overview</h1>
          <p className="text-zinc-500 text-sm font-medium uppercase tracking-wider">
            Real-time dashboard for managing active and past incidents.
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" className="bg-transparent border-zinc-800 text-zinc-300">
            <Download className="w-4 h-4 mr-2" /> Export
          </Button>
          <Button className="bg-blue-600 hover:bg-blue-500 text-white font-bold px-6">
            <Plus className="w-4 h-4 mr-2" /> Create Incident
          </Button>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {[
          { label: "CRITICAL ACTIVE", value: criticalCount, icon: AlertCircle, color: "text-red-500", trend: "+1" },
          { label: "TOTAL OPEN", value: reports.length, icon: FolderOpen, color: "text-blue-500" },
          { label: "AVG. ACK TIME", value: "4m 30s", icon: Clock, color: "text-emerald-500", trend: "-12%" },
          { label: "UNASSIGNED", value: unassignedCount, icon: UserMinus, color: "text-orange-500" },
        ].map((stat, i) => (
          <div key={i} className="bg-zinc-900/40 border border-zinc-800 p-6 rounded-xl flex justify-between items-start">
            <div className="space-y-2">
              <p className="text-[10px] font-bold text-zinc-500 uppercase tracking-widest">{stat.label}</p>
              <div className="flex items-center gap-2">
                <span className="text-3xl font-bold">{stat.value}</span>
                {stat.trend && (
                  <span className={`text-[10px] font-bold px-1.5 py-0.5 rounded ${stat.trend.startsWith('+') ? 'bg-red-500/10 text-red-500' : 'bg-emerald-500/10 text-emerald-500'}`}>
                    {stat.trend}
                  </span>
                )}
              </div>
            </div>
            <stat.icon className={`w-10 h-10 ${stat.color} opacity-20`} />
          </div>
        ))}
      </div>

      {/* Filter Bar */}
      <div className="bg-zinc-900/20 border border-zinc-800 p-4 rounded-xl flex flex-wrap gap-4 items-center">
        <div className="relative flex-1 min-w-[300px]">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
          <Input 
            placeholder="Search by ID, title, affected service..." 
            className="bg-zinc-900/50 border-zinc-800 pl-10 h-10 text-sm"
          />
        </div>
        <Select defaultValue="all">
          <SelectTrigger className="w-[140px] bg-zinc-900/50 border-zinc-800 h-10 text-sm">
            <SelectValue placeholder="Severity" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Severity: All</SelectItem>
            <SelectItem value="high">Critical</SelectItem>
            <SelectItem value="low">Low</SelectItem>
          </SelectContent>
        </Select>
        <Select defaultValue="active">
          <SelectTrigger className="w-[140px] bg-zinc-900/50 border-zinc-800 h-10 text-sm">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="active">Status: Active</SelectItem>
            <SelectItem value="resolved">Resolved</SelectItem>
          </SelectContent>
        </Select>
        <Button variant="outline" className="bg-zinc-900/50 border-zinc-800 h-10">
          <Calendar className="w-4 h-4 mr-2" /> Date Range
        </Button>
        <Button variant="ghost" size="icon" className="border border-zinc-800"><Filter className="w-4 h-4" /></Button>
      </div>

      {/* Table Container */}
      <div className="border border-zinc-800 rounded-xl bg-zinc-900/20 overflow-hidden">
        <table className="w-full text-left">
          <thead>
            <tr className="bg-zinc-900/40 border-b border-zinc-800 text-[10px] font-bold text-zinc-500 uppercase tracking-widest">
              <th className="px-6 py-4">ID</th>
              <th className="px-6 py-4">Incident Title</th>
              <th className="px-6 py-4">Severity</th>
              <th className="px-6 py-4">Status</th>
              <th className="px-6 py-4">Location</th>
              <th className="px-6 py-4">Owner</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-800/50">
            {reports.map((report) => (
              <tr key={report.id} className="hover:bg-zinc-800/20 transition-colors group">
                <td className="px-6 py-5 text-sm font-medium text-zinc-500">#INC-{report.id}</td>
                <td className="px-6 py-5">
                  <div className="space-y-0.5">
                    <p className="text-sm font-semibold">{report.title}</p>
                    <p className="text-xs text-zinc-500 truncate max-w-[250px]">{report.narrative}</p>
                  </div>
                </td>
                <td className="px-6 py-5">
                  <Badge className={`rounded-full px-3 py-0.5 border font-semibold text-[10px] uppercase ${getSeverityStyle(report.impact)}`}>
                    • {report.impact}
                  </Badge>
                </td>
                <td className="px-6 py-5">
                  <Badge className={`rounded-md px-3 py-1 border font-medium text-xs ${getStatusStyle(report.status)}`}>
                    {report.status}
                  </Badge>
                </td>
                <td className="px-6 py-5">
                  <div className="flex items-center gap-2 text-zinc-400">
                    <Globe className="w-4 h-4" />
                    <span className="text-xs font-medium">{report.location}</span>
                  </div>
                </td>
                <td className="px-6 py-5">
                   <div className="flex items-center gap-2">
                     <div className="w-6 h-6 rounded-full bg-zinc-800 border border-zinc-700 flex items-center justify-center text-[10px] text-zinc-500 font-bold">
                       {report.assignedTo ? report.assignedTo[0] : "?"}
                     </div>
                   </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {/* Footer / Pagination */}
        <div className="bg-zinc-900/40 border-t border-zinc-800 px-6 py-4 flex items-center justify-between text-xs text-zinc-500">
          <p>Showing <span className="text-zinc-200 font-medium">1 to {reports.length}</span> of <span className="text-zinc-200 font-medium">{reports.length}</span> results</p>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" className="h-8 bg-zinc-900/50 border-zinc-800 text-zinc-400">Previous</Button>
            <Button variant="outline" size="sm" className="h-8 bg-zinc-900 border-zinc-700 text-zinc-200">Next</Button>
          </div>
        </div>
      </div>
    </div>
  );
}