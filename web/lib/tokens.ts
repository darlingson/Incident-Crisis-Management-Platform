/**
 * CrisisCommand — Minimal Corporate Tokens
 * Single source: globals.css :root vars → Tailwind via @theme inline
 * Framework: Tailwind 4 + shadcn/ui (Radix) — use these, no bg-[#...] hardcodes
 * Feel: light, minimal, corporate (white canvas, slate neutrals, one blue)
 */

export const tokens = {
  color: {
    // Base — light minimal
    background: "oklch(0.991 0 0)", // #f8fafc
    foreground: "oklch(0.21 0.02 256)", // slate-900
    card: "oklch(1 0 0)", // white
    border: "oklch(0.93 0.01 256)", // slate-200 #e2e8f0
    muted: "oklch(0.97 0.005 256)", // slate-100
    mutedForeground: "oklch(0.551 0.015 256)", // slate-500
    primary: "oklch(0.55 0.22 255)", // #2563eb corporate
    primaryForeground: "oklch(0.985 0 0)",
    destructive: "oklch(0.6 0.22 25)", // #dc2626
    // Incident semantics — one mapping
    severity: {
      critical: "var(--severity-critical)", // destructive/5%
      medium: "oklch(0.66 0.18 35)", // amber-600
      low: "oklch(0.55 0.015 256)", // slate-500
    },
    status: {
      reported: "var(--status-reported)", // primary/10
      investigation: "oklch(0.55 0.15 280)", // violet
      resolved: "oklch(0.6 0.15 150)", // emerald
    },
  },
  radius: {
    sm: "calc(var(--radius) - 4px)",
    md: "calc(var(--radius) - 2px)",
    lg: "var(--radius)", // 0.5rem — corporate minimal
    xl: "calc(var(--radius) + 4px)",
  },
  font: {
    sans: "var(--font-geist-sans)",
    monoFamily: "var(--font-geist-mono)",
    // Minimal type scale
    h1: "text-2xl font-semibold tracking-tight", // not 5xl extrabold
    h2: "text-xl font-semibold",
    label: "text-xs font-medium text-muted-foreground",
    body: "text-sm text-foreground",
    mono: "font-mono text-xs tracking-tight",
  },
  space: {
    // 8pt grid only
    card: "p-6", // not p-3/5
    section: "space-y-6", // not space-y-8/10 mix
    gap: "gap-6",
  },
  icon: {
    size: "w-4 h-4", // 16px default, not 6 vs 5 mix
    stroke: "stroke-[1.7]",
    // Single meaning
    map: {
      incident: "ShieldCheck",
      report: "FileText",
      service: "Activity",
      security: "ShieldCheck",
    },
  },
  component: {
    card: "bg-card border border-border shadow-sm rounded-lg",
    button: {
      primary: "bg-primary text-primary-foreground hover:bg-primary/90",
      outline: "border border-border bg-background hover:bg-muted",
      ghost: "text-muted-foreground hover:bg-muted hover:text-foreground",
    },
    badge: {
      severity: "border bg-card",
      status: "variant-outline",
    },
  },
} as const;
