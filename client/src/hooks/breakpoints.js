import useBreakpoint from "use-breakpoint";

const config = { xs: 0, sm: 576, md: 768, lg: 992, xl: 1200 };

function useXs() {
  const { breakpoint } = useBreakpoint(config, "xs");
  return breakpoint === "xs";
}

function useSm() {
  const { breakpoint } = useBreakpoint(config, "xs");
  return breakpoint === "sm";
}

export function useXsOrSm() {
  const xs = useXs();
  const sm = useSm();
  return xs || sm;
}

export function useNotLarge(){
  const { breakpoint } = useBreakpoint(config, "md");
  const xsOrSm = useXsOrSm();
  return breakpoint === "md" || xsOrSm;
}