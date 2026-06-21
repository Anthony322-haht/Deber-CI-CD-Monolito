/** Configuración inyectada desde Login.aspx (script inline). */
interface LoginFormConfig {
  ids: {
    user: string;
    pass: string;
    submit: string;
  };
  msgUsuario: string;
  msgClave: string;
  msgReenvio: string;
  swalConfirmColor: string;
}

interface Window {
  loginFormConfig?: LoginFormConfig;
  /** Evita doble envío / reenvío del formulario. */
  __loginSubmitting?: boolean;
  togglePassword?: (inputId: string) => void;
  handleLoginSubmit?: () => boolean;
}

/** SweetAlert2 cargado por CDN en Login.aspx */
declare const Swal: {
  fire: (options: Record<string, unknown>) => Promise<unknown>;
};
